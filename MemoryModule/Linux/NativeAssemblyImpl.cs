using GlibcInterop;
using MemoryModule.Linux.Elf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryModule.Linux
{
    internal static unsafe class NativeAssemblyImpl
    {
        /// <summary>
        /// Load EXE/DLL from memory location with the given size.
        /// All dependencies are resolved using default LoadLibrary/GetProcAddress
        /// calls through the Windows API, or through passed delegates.
        /// </summary>
        /// <param name="data">The assembly's code</param>
        /// <param name="length">The assembly's code's length</param>
        /// <returns>A handle to the loaded assembly</returns>
        public static IntPtr LoadLibrary(
            byte* dataPtr,
            long length,
            CustomAllocFunc allocMemory = null,
            CustomFreeFunc freeMemory = null,
            CustomLoadLibraryFunc loadLibrary = null,
            CustomGetProcAddressFunc getProcAddress = null,
            CustomFreeLibraryFunc freeLibrary = null
        )
        {
            allocMemory = allocMemory ?? MemoryDefaultAllocDelegate;
            freeMemory = freeMemory ?? MemoryDefaultFreeDelegate;
            loadLibrary = loadLibrary ?? MemoryDefaultLoadLibraryDelegate;
            getProcAddress = getProcAddress ?? MemoryDefaultGetProcAddressDelegate;
            freeLibrary = freeLibrary ?? MemoryDefaultFreeLibraryDelegate;

            return (IntPtr)MemoryLoadLibraryEx(
                dataPtr,
                (ulong)length,
                allocMemory,
                freeMemory,
                loadLibrary,
                getProcAddress,
                freeLibrary,
                null
            );
        }

        /// <summary>
        /// Free previously loaded EXE/DLL.
        /// </summary>
        /// <param name="handle"></param>
        public static bool FreeLibrary(IntPtr handle)
        {
            return MemoryFreeLibrary(handle);
        }

        public static IntPtr GetSymbol(IntPtr module, string name)
        {
            char* namePtr = null;
            try
            {
                namePtr = (char*)Marshal.StringToHGlobalAnsi(name);
                return (IntPtr)MemoryGetProcAddress(module, namePtr);
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)namePtr);
            }
        }

        public static void* GetSymbolUnsafe(void* module, char* name)
        {
            return MemoryGetProcAddress((IntPtr)module, name);
        }

        // To ensure that the GC doesn't fuck up our delegates.
        public static CustomAllocFunc MemoryDefaultAllocDelegate = MemoryDefaultAlloc;
        public static CustomFreeFunc MemoryDefaultFreeDelegate = MemoryDefaultFree;
        public static CustomLoadLibraryFunc MemoryDefaultLoadLibraryDelegate = MemoryDefaultLoadLibrary;
        public static CustomGetProcAddressFunc MemoryDefaultGetProcAddressDelegate = MemoryDefaultGetProcAddress;
        public static CustomFreeLibraryFunc MemoryDefaultFreeLibraryDelegate = MemoryDefaultFreeLibrary;

        #region Default dependency resolvers
        internal static void* MemoryDefaultAlloc(void* address, UIntPtr size, MemoryAllocation allocationType, PageProtection protect, void* userdata)
        {
            return mmap(address, size, Helpers.WindowsToUnixProtection(protect), MmapMappingFlags.Anonymous | MmapMappingFlags.Private);
        }

        internal static bool MemoryDefaultFree(void* lpAddress, UIntPtr dwSize, MemoryAllocation dwFreeType, void* userdata)
        {
            return munmap(lpAddress, dwSize) == 0;
        }

        internal static void* MemoryDefaultLoadLibrary(char* filename, void* userdata)
        {
            void* result;
            result = dlopen(filename);
            return result;
        }

        internal static void* MemoryDefaultGetProcAddress(void* module, char* name, void* userdata)
        {
            return dlsym(module, name);
        }

        internal static bool MemoryDefaultFreeLibrary(void* module, void* userdata)
        {
            return dlclose(module) == 0;
        }
        #endregion

        private static readonly uint HostMachine =
            (uint)(Environment.Is64BitProcess ?
            ElfArchitecture.x86_64 :
            ElfArchitecture.x86);

        #region Core C functions
        private static void* MemoryLoadLibrary(void* data, ulong size)
        {
            return MemoryLoadLibraryEx(
                data,
                size,
                MemoryDefaultAllocDelegate,
                MemoryDefaultFreeDelegate,
                MemoryDefaultLoadLibraryDelegate,
                MemoryDefaultGetProcAddressDelegate,
                MemoryDefaultFreeLibraryDelegate,
                null);
        }

        private static void* MemoryLoadLibraryEx(void* data, ulong size,
            CustomAllocFunc allocMemory,
            CustomFreeFunc freeMemory,
            CustomLoadLibraryFunc loadLibrary,
            CustomGetProcAddressFunc getProcAddress,
            CustomFreeLibraryFunc freeLibrary,
            void* userdata)
        {
            var result = new MemoryModule();
            var gcHandle = GCHandle.Alloc(result);
            var resultHandle = (IntPtr)gcHandle;

            try
            {
                CheckSize(size, (ulong)sizeof(ElfHeaderNative));

                var header = (ElfHeaderNative*)data;
                if (header->Magic[0] != 0x7F ||
                    header->Magic[1] != (byte)'E' ||
                    header->Magic[2] != (byte)'L' ||
                    header->Magic[3] != (byte)'F'
                    )
                {
                    throw new NativeAssemblyLoadException("Bad image format: Wrong magic number.");
                }

                if ((uint)header->Machine != HostMachine)
                {
                    throw new NativeAssemblyLoadException("Bad image format: Wrong architecture.");
                }

                if (header->Type != ElfObjectType.Dyn)
                {
                    throw new NativeAssemblyLoadException("Only ELF dynamic shared objects are supported.");
                }

                if (header->ProgramHeaderSize != sizeof(ElfProgramHeaderNative))
                {
                    throw new NativeAssemblyLoadException("Invalid ELF program header size.");
                }

                if (header->SectionHeaderSize != sizeof(ElfSectionHeaderNative))
                {
                    throw new NativeAssemblyLoadException("Invalid ELF section header size.");
                }

                var managedHeader = new ElfHeader((byte*)data);

                result.alloc = allocMemory;
                result.free = freeMemory;
                result.getProcAddress = getProcAddress;
                result.loadLibrary = loadLibrary;
                result.freeLibrary = freeLibrary;
                result.userdata = userdata;
                result.pageSize = (ulong)sysconf(_SC_PAGE_SIZE);

                AllocateMemory(result, managedHeader);

                HandleTLS(result, managedHeader);

                ResolveSymbols(result, managedHeader);

                ResolveDependencies(result, managedHeader);

                ApplyRelocations(result, managedHeader);

                Initialize(result, managedHeader);

                StoreFinalizers(result, managedHeader);

                return (void *)resultHandle;
            }
            catch (NativeAssemblyLoadException)
            {
                MemoryFreeLibrary(resultHandle);
                throw;
            }
            catch (Exception e)
            {
                MemoryFreeLibrary(resultHandle);
                throw new NativeAssemblyLoadException("Failed to load assembly", e);
            }
        }
        private static bool MemoryFreeLibrary(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return true;
            }

            var gcHandle = GCHandle.FromIntPtr(handle);
            var module = (MemoryModule)gcHandle.Target;

            if (module == null)
            {
                return true;
            }

            if (module.initialized)
            {
                Finalize(module);
            }

            if (module.dependencies != null)
            {
                // free previously opened libraries
                foreach (var dep in module.dependencies)
                {
                    if (dep != null)
                    {
                        module.freeLibrary((void *)dep, module.userdata);
                    }
                }
            }

            if (module.codeBase != null)
            {
                // release memory of library
                module.free(module.codeBase, (UIntPtr)0, MemoryAllocation.Release, module.userdata);
            }

            if (module.symbolArrayPtr != null)
            {
                CStyleMemory.free(module.symbolArrayPtr);
            }

            if (module.map != null)
            {
                GlibcTls.RemoveFromSlotInfo(module.map);
                CStyleMemory.free(module.map.TlsInitialImage);
                module.map.Dispose();
            }

            gcHandle.Free();

            return true;
        }

        private static void* MemoryGetProcAddress(IntPtr handle, char* name)
        {
            var str = Marshal.PtrToStringAnsi((IntPtr)name);
            var gcHandle = GCHandle.FromIntPtr(handle);
            var module = gcHandle.Target as MemoryModule;

            if (module == null)
            {
                return null;
            }

            if (module.nameExportsTable.TryGetValue(str, out var symbols))
            {
                return (void *)(symbols.FirstOrDefault()?.Value ?? 0);
            }

            return null;
        }
        #endregion

        #region Helpers
        private static void CheckSize(ulong size, ulong expected)
        {
            if (size < expected)
            {
                throw new NativeAssemblyLoadException("Bad image format.");
            }
        }
        #endregion

        #region Functions that should have been macros
        #endregion

        #region Functions that the author marks as inline
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong AlignValueUp(ulong value, ulong alignment)
        {
            return (value + alignment - 1) & ~(alignment - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void* OffsetPointer(void* data, int offset)
        {
            return Unsafe.Add<byte>(data, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong AlignValueDown(ulong value, ulong alignment)
        {
            return value & ~(alignment - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void* AlignAddressDown(void* address, UIntPtr alignment)
        {
            return (void*)AlignValueDown((ulong)address, (ulong)alignment);
        }
        #endregion

        private static void AllocateMemory(MemoryModule module, ElfHeader managedHeader)
        {
            ulong lowAddress = ulong.MaxValue;
            ulong highAddress = ulong.MinValue;

            var needsLoading =
                 managedHeader.ProgramHeaders
                    .Where(header => header.Type == ElfProgramHeaderType.Load).ToList();

            if (needsLoading.Count == 0)
            {
                return;
            }

            foreach (var header in needsLoading)
            {
                ulong currentLowAddress = header.VirtualAddress;
                ulong currentHighAddress = currentLowAddress + header.MemorySize;

                lowAddress = Math.Min(lowAddress, currentLowAddress);
                highAddress = Math.Max(highAddress, currentHighAddress);
            }

            var alignedSize = AlignValueUp(highAddress - lowAddress, module.pageSize);
            var baseAddress = module.alloc((void*)lowAddress, (UIntPtr)alignedSize, MemoryAllocation.Reserve, PageProtection.WriteCopy, module.userdata);

            if (baseAddress == null)
            {
                throw new OutOfMemoryException("Failed to allocated memory for program headers.");
            }

            module.codeBase = baseAddress;
            module.codeSize = alignedSize;

            foreach (var header in needsLoading)
            {
                ulong currentLowAddress = header.VirtualAddress;
                ulong currentHighAddress = currentLowAddress + header.MemorySize;

                var trueAddress = (byte*)baseAddress + currentLowAddress - lowAddress;

                var alignedAddress = AlignValueDown((ulong)trueAddress, module.pageSize);

                CStyleMemory.memcpy(trueAddress, managedHeader.GetAddress(header.Offset), (uint)header.FileSize);

                var mmapFlags = MmapProtectionFlags.None;

                var programFlags = header.Flags;

                if ((programFlags & (int)ElfProgramHeaderLoadFlags.Execute) != 0)
                {
                    mmapFlags |= MmapProtectionFlags.Execute;
                }
                if ((programFlags & (int)ElfProgramHeaderLoadFlags.Read) != 0)
                {
                    mmapFlags |= MmapProtectionFlags.Read;
                }
                if ((programFlags & (int)ElfProgramHeaderLoadFlags.Write) != 0)
                {
                    mmapFlags |= MmapProtectionFlags.Write;
                }

                if (mprotect(
                    (void *)alignedAddress, 
                    (UIntPtr)((ulong)header.MemorySize + (ulong)trueAddress - alignedAddress), 
                    mmapFlags) != IntPtr.Zero)
                {
                    throw new NativeAssemblyLoadException($"Cannot protect memory at: 0x{alignedAddress:x}");
                }
            }
        }

        private static void HandleTLS(MemoryModule module, ElfHeader managedHeader)
        {
            var tlsSection = managedHeader.ProgramHeaders
                .FirstOrDefault(h => h.Type == ElfProgramHeaderType.ThreadLocalStorage);

            if (tlsSection == null)
            {
                return;
            }

            module.map = new LinkMap();
            module.map.TlsBlockAlignment = tlsSection.Align;
            module.map.TlsBlockSize = tlsSection.MemorySize;
            module.map.TlsInitImageSize = tlsSection.FileSize;

            // We need to allocate a new array, as the given memory pointer
            // does not need to be valid after the library has loaded.
            var tlsImagePtr = (byte*)CStyleMemory.malloc((uint)tlsSection.FileSize);
            var tlsImageInFile = managedHeader.GetAddress(tlsSection.Offset);

            CStyleMemory.memcpy(tlsImagePtr, tlsImageInFile, (uint)tlsSection.FileSize);

            module.map.TlsInitialImage = tlsImagePtr;

            var modId = GlibcTls.GetNextModuleId();
            module.map.TlsModuleId = modId;

            GlibcTls.AddToSlotInfo(module.map);
        }

        private static void ResolveDependencies(MemoryModule module, ElfHeader managedHeader)
        {
            var dynSection = managedHeader.DynamicSectionHeader;

            // For performance reasons.
            var items = dynSection.DynamicSectionItems.ToList();

            var strTable = items.First(item => item.Tag == ElfDynamicSectionItemType.StrTab);

            var deps = new List<IntPtr>();

            foreach (var item in items.Where(item => item.Tag == ElfDynamicSectionItemType.Needed))
            {
                var name = managedHeader.GetAddress((ulong)strTable.Pointer + item.Value);
                var addr = module.loadLibrary((char *)name, module.userdata);
                if (addr == null)
                {
                    module.dependencies = deps.ToArray();
                    throw new NativeAssemblyLoadException($"Cannot load dependency: {Marshal.PtrToStringAnsi((IntPtr)name)}");
                }
                deps.Add((IntPtr)addr);
            }

            module.dependencies = deps.ToArray();
        }

        private static void ResolveSymbols(MemoryModule module, ElfHeader managedHeader)
        {
            var dynSection = managedHeader.DynamicSectionHeader;

            // For performance reasons.
            var items = dynSection.DynamicSectionItems.ToList();
            var symbolTable = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.SymTab);

            if (symbolTable == null)
            {
                module.symbolArray = null;
                module.symbolArrayPtr = null;
                module.nameExportsTable = null;
            }

            var symbolTableSection = managedHeader.SectionHeaders.First(header => header.Offset == symbolTable.Value);

            Debug.Assert(symbolTableSection.EntrySize == (ulong)Marshal.SizeOf<ElfSymbolNative>());

            // We must keep a copy of the symbol array.
            // It is important for future symbol lookup.
            module.symbolArrayPtr = CStyleMemory.malloc((uint)symbolTableSection.Size);

            CStyleMemory.memcpy(
                module.symbolArrayPtr,
                managedHeader.GetAddress(symbolTableSection.Offset),
                (uint)symbolTableSection.Size);

            var symbolArray = module.symbolArray =
                new ElfSymbolArray((byte*)module.symbolArrayPtr, 0, symbolTableSection.Size / symbolTableSection.EntrySize);

            var strTable = items.First(item => item.Tag == ElfDynamicSectionItemType.StrTab);
            var stringTable = new ElfStringTable(managedHeader.GetAddress(), strTable.Value);

            module.nameExportsTable = new Dictionary<string, List<ElfSymbol>>();

            foreach (var symbol in symbolArray)
            {
                var name = symbol.ResolveName(stringTable);
                if (symbol.Type != ElfSymbolType.TLS && symbol.Value != 0)
                {
                    // To-Do: Resolve the actual value taking alignment into consideration.
                    symbol.Value += (ulong)module.codeBase;
                }
                if (module.nameExportsTable.ContainsKey(name))
                {
                    module.nameExportsTable[name].Add(symbol);
                }
                else
                {
                    module.nameExportsTable.Add(name, new List<ElfSymbol> { symbol });
                }
            }
        }

        private static void ApplyRelocations(MemoryModule module, ElfHeader managedHeader)
        {
            var dynSection = managedHeader.DynamicSectionHeader;

            var items = dynSection.DynamicSectionItems.ToList();
            var relaTable = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.Rela);
            var relaSize = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.RelaSz);
            var relaEntrySize = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.RelaEnt);

            if (relaEntrySize != null)
            {
                Debug.Assert(relaEntrySize.Value == (ulong)sizeof(ElfRelaNative));
            }

            var relaArr = relaTable == null ? Enumerable.Empty<ElfRela>() :
                new ElfRelaArray(managedHeader.GetAddress(), relaTable.Value, relaSize.Value / relaEntrySize.Value);

            var jumpRel = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.JmpRel);

            var jumpRelaArr = Enumerable.Empty<ElfRela>();

            if (jumpRel != null)
            {
                var jumpRelSection = managedHeader.SectionHeaders.FirstOrDefault(header => header.Offset == jumpRel.Value);
                jumpRelaArr = new ElfRelaArray(managedHeader.GetAddress(), jumpRelSection.Offset, jumpRelSection.Size / jumpRelSection.EntrySize);
            }

            foreach (var rela in relaArr.Concat(jumpRelaArr))
            {
                var baseAddress = (byte*)module.codeBase;
                var affected = (void**)(baseAddress + rela.Offset);

                var symbolHandle = IntPtr.Zero;

                if (rela.DependsOnSymbol())
                {
                    symbolHandle = LookupSymbol(module, rela);
                }

                if (rela.Type.IsNone())
                {
                    continue;
                }
                else if (rela.Type.IsRelative())
                {
                    *affected = baseAddress + rela.Addend;
                }
                else if (rela.Type.IsGlobDat() || rela.Type.IsJmpSlot() || rela.Type.Is64())
                {
                    *affected = (void*)((ulong)symbolHandle + rela.Addend);
                }
                else if (rela.Type.Is16())
                {
                    var affected16 = (ushort*)affected;
                    *affected16 = (ushort)((ulong)symbolHandle + rela.Addend);
                }
                else if (rela.Type.IsModule())
                {
                    *affected = (void*)module.map.TlsModuleId;
                }
                else if (rela.Type.IsOffset())
                {
                    var currentSymbol = module.symbolArray[rela.Symbol];
                    *affected = (void*)currentSymbol.Value;
                }
                else
                {
                    throw new NotImplementedException($"Unimplemented relocation type: {rela.Type}");
                }
            }
        }

        private static IntPtr LookupSymbol(MemoryModule module, ElfRela rela)
        {
            var sym = module.symbolArray[rela.Symbol];

            if (sym.Value != 0)
            {
                return (IntPtr)sym.Value;
            }

            foreach (var dep in module.dependencies)
            {
                IntPtr str = Marshal.StringToHGlobalAnsi(sym.Name);
                try
                {
                    var addr = module.getProcAddress((void*)dep, (char*)str, module.userdata);
                    if (addr != null)
                    {
                        sym.Value = (ulong)addr;
                        return (IntPtr)sym.Value;
                    }

                }
                finally
                {
                    Marshal.FreeHGlobal(str);
                }
            }

            if (sym.Binding != ElfSymbolBinding.Weak)
            {
                throw new NativeAssemblyLoadException($"Cannot load assembly: Failed to resolve symbol: {sym.Name}");
            }

            return IntPtr.Zero;
        }

        private static void Initialize(MemoryModule module, ElfHeader managedHeader)
        {
            var items = managedHeader.DynamicSectionHeader.DynamicSectionItems.ToList();
            var initItem = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.Init);

            // No constructors.
            if (initItem == null)
            {
                module.initialized = true;
                return;
            }

            var initArrItem = items.First(item => item.Tag == ElfDynamicSectionItemType.InitArray);
            var initArrSizeItem = items.First(item => item.Tag == ElfDynamicSectionItemType.InitArraySz);
            var initArrCount = initArrSizeItem.Value / (ulong)sizeof(IntPtr);

            var args = Environment.GetCommandLineArgs();
            var env = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Select(x => $"{x.Key}={x.Value}");

            int argc = args.Length;
            var argArr = args.Select(arg => Marshal.StringToHGlobalAnsi(arg)).ToArray();
            var envArr = env.Select(envv => Marshal.StringToHGlobalAnsi(envv))
                .Concat(new[] { IntPtr.Zero })
                .ToArray();

            try
            {
                var firstInitDelegate = Marshal.GetDelegateForFunctionPointer<InitDelegate>
                    ((IntPtr)((byte *)module.codeBase + initItem.Value));

                fixed (IntPtr* argv = &argArr[0])
                {
                    fixed (IntPtr* envp = &envArr[0])
                    {
                        firstInitDelegate(argc, (byte**)argv, (byte**)envp);

                        var initPtrs = new IntPtr[initArrCount];

                        // Must be the real codeBase, not the file.
                        Marshal.Copy((IntPtr)((ulong)module.codeBase + initArrItem.Value), initPtrs, 0, (int)initArrCount);

                        foreach (var initPtr in initPtrs)
                        {
                            // These functions are called AFTER relocation, therefore,
                            // they must not be manually "relocated" by adding to the module.codeBase
                            var initDelegate = Marshal.GetDelegateForFunctionPointer<InitDelegate>(initPtr);
                            initDelegate(argc, (byte**)argv, (byte**)envp);
                        }
                    }
                }

                module.initialized = true;
            }
            finally
            {
                foreach (var arg in argArr)
                {
                    Marshal.FreeHGlobal(arg);
                }

                foreach (var envptr in envArr)
                {
                    Marshal.FreeHGlobal(envptr);
                }
            }
        }

        private static void StoreFinalizers(MemoryModule module, ElfHeader managedHeader)
        {
            var items = managedHeader.DynamicSectionHeader.DynamicSectionItems.ToList();
            var finiItem = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.Fini);
            var finiArrayItem = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.FiniArray);
            var finiArraySizeItem = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.FiniArraySz);

            // The finalizers should have been copied, along with the program headers.
            module.firstFinalizer = (IntPtr)finiItem.Value;
            module.finalizerArray = (IntPtr)finiArrayItem.Value;
            module.finalizerArrayCount = finiArraySizeItem.Value / (ulong)sizeof(IntPtr);
        }

        private static void Finalize(MemoryModule module)
        {
            var firstFiniPtr = (IntPtr)((ulong)module.codeBase + (ulong)module.firstFinalizer);
            var firstFini = Marshal.GetDelegateForFunctionPointer<FiniDelegate>(firstFiniPtr);
            firstFini();

            IntPtr* finiArr = (IntPtr*)((ulong)module.codeBase + (ulong)module.finalizerArray);

            for (ulong i = 0; i < module.finalizerArrayCount; ++i)
            {
                var fini = Marshal.GetDelegateForFunctionPointer<FiniDelegate>(finiArr[i]);
                fini();
            }
        }

        #region Nightmare
        class MemoryModule
        {
            public void* codeBase;
            public ulong codeSize;
            public LinkMap map;
            public unsafe IntPtr[] dependencies;
            public bool initialized;
            public CustomAllocFunc alloc;
            public CustomFreeFunc free;
            public CustomLoadLibraryFunc loadLibrary;
            public CustomGetProcAddressFunc getProcAddress;
            public CustomFreeLibraryFunc freeLibrary;
            // There may... be some duplicate entries. Watch out.
            public Dictionary<string, List<ElfSymbol>> nameExportsTable;
            public void* symbolArrayPtr;
            public ElfSymbolArray symbolArray;
            public IntPtr firstFinalizer;
            public IntPtr finalizerArray;
            public ulong finalizerArrayCount;
            public unsafe void* userdata;
            public ulong pageSize;
        }

        [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 4)]
        public struct POINTER_LIST
        {
            public unsafe POINTER_LIST* next;
            public unsafe void* address;
        }
        #endregion

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void InitDelegate(int argc, byte** argv, byte** envp);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void FiniDelegate();

        #endregion

        #region P/Invoke
        private const int _SC_PAGESIZE = 30;
        private const int _SC_PAGE_SIZE = 30;

        [DllImport("libc")]
        private static extern void* mmap(
            void* addr, 
            UIntPtr length, 
            MmapProtectionFlags protectionFlags, 
            MmapMappingFlags mappingFlags, 
            int fileDescriptor = -1, int offset = 0);

        [DllImport("libc")]
        private static extern IntPtr mprotect(void* addr, UIntPtr length, MmapProtectionFlags protectionFlags);

        [DllImport("libc")]
        private static extern int munmap(void* addr, UIntPtr length);

        // For page size.
        [DllImport("libc")]
        private static extern IntPtr sysconf(int name);

        [DllImport("dl")]
        private static extern void* dlopen(char* name, int mode = 0x01 | 0x00100 /*RTLD_LAZY | RTLD_GLOBAL*/);

        [DllImport("dl")]
        private static extern void* dlsym(void* handle, char* name);

        [DllImport("dl")]
        private static extern int dlclose(void* handle);

        #endregion
    }
}