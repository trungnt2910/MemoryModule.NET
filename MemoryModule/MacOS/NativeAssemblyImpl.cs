using DyldInterop;
using MemoryModule.MacOS.Macho;
using MemoryModule.MacOS.Macho.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryModule.MacOS
{
    /// <summary>
    /// Loads a native Macho assembly to the process.
    /// This implementation is different from the one from Windows and Linux, where the assembly is loaded in managed C# code.
    /// Here, the assembly is loaded directly using dyld's internal API, and the Macho parser library is included solely to inspect 
    /// dyld.
    /// We might have loaded the library ourselves, but support for TLS would be much harder (dyld does not expose anything).
    /// </summary>
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
                CheckSize(size, (ulong)sizeof(MachoHeaderNative));

                var header = (MachoHeaderNative*)data;
                if (header->magic != MachoMagic.Macho32Bit && header->magic != MachoMagic.Macho64Bit)
                {
                    throw new NativeAssemblyLoadException("Bad image format: Wrong magic number.");
                }

                if (CpuTypeToArchitecture(header->cputype) != RuntimeInformation.OSArchitecture)
                {
                    throw new NativeAssemblyLoadException("Bad image format: Wrong architecture.");
                }

                if (header->filetype != MachoFileType.Bundle && header->filetype != MachoFileType.Dylib)
                {
                    throw new NativeAssemblyLoadException("Only Macho dynamic objects and bundles are supported.");
                }

                var managedHeader = new MachoHeader((byte*)data);

                result.alloc = allocMemory;
                result.free = freeMemory;
                result.getProcAddress = getProcAddress;
                result.loadLibrary = loadLibrary;
                result.freeLibrary = freeLibrary;
                result.userdata = userdata;
                result.pageSize = (ulong)sysconf(_SC_PAGESIZE);

                ResolveDependencies(result, managedHeader);

                result.nativeHandle = Dyld.Load((byte *)data, size, managedHeader.Dylibs[0].Name);

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

            if (module.nativeHandle != IntPtr.Zero)
            {
                Dyld.Unload(handle);
            }

            gcHandle.Free();

            return true;
        }

        private static void* MemoryGetProcAddress(IntPtr handle, char* name)
        {
            var gcHandle = GCHandle.FromIntPtr(handle);
            var module = gcHandle.Target as MemoryModule;

            if (module == null)
            {
                return null;
            }

            return (void*)Dyld.Sym(module.nativeHandle, name);
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

        private static Architecture? CpuTypeToArchitecture(MachoCpuType type)
        {
            switch (type)
            {
                case MachoCpuType.ARM:
                case MachoCpuType.ARM64_32:
                    return Architecture.Arm;
                case MachoCpuType.ARM64:
                    return Architecture.Arm64;
                case MachoCpuType.I386:
                    return Architecture.X86;
                case MachoCpuType.X86_64:
                    return Architecture.X64;
                default:
                    return null;
            }
        }
        #endregion

        private static void ResolveDependencies(MemoryModule module, MachoHeader managedHeader)
        {
            var dylibs = managedHeader.Dylibs;

            var deps = new List<IntPtr>();

            // First dylib is itself, so skip it.
            foreach (var item in dylibs.Skip(1))
            {
                var name = item.NamePtr;
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

        #region Nightmare
        class MemoryModule
        {
            public void* codeBase;
            public ulong codeSize;
            public unsafe IntPtr[] dependencies;
            public IntPtr nativeHandle;
            public CustomAllocFunc alloc;
            public CustomFreeFunc free;
            public CustomLoadLibraryFunc loadLibrary;
            public CustomGetProcAddressFunc getProcAddress;
            public CustomFreeLibraryFunc freeLibrary;
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
        private const int _SC_PAGESIZE = 29;

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

        [DllImport("/usr/lib/libSystem.dylib")]
        private static extern void* dlopen(char* name, int mode = 0x01 | 0x00100 /*RTLD_LAZY | RTLD_GLOBAL*/);

        [DllImport("/usr/lib/libSystem.dylib")]
        private static extern void* dlsym(void* handle, char* name);

        [DllImport("/usr/lib/libSystem.dylib")]
        private static extern int dlclose(void* handle);

        #endregion
    }
}