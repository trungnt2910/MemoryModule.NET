// COPYRIGHT NOTICE FROM MEMORYMODULE
// This copyright notice below applies for the contents of this file.

/*
 * Memory DLL loading code
 * Version 0.0.4
 *
 * Copyright (c) 2004-2015 by Joachim Bauch / mail@joachim-bauch.de
 * http://www.joachim-bauch.de
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 2.0 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is MemoryModule.c
 *
 * The Initial Developer of the Original Code is Joachim Bauch.
 *
 * Portions created by Joachim Bauch are Copyright (C) 2004-2015
 * Joachim Bauch. All Rights Reserved.
 *
 *
 * THeller: Added binary search in MemoryGetProcAddress function
 * (#define USE_BINARY_SEARCH to enable it).  This gives a very large
 * speedup for libraries that exports lots of functions.
 *
 * These portions are Copyright (C) 2013 Thomas Heller.
 */

// Any other files are subject to the copyright notice
// in the LICENSE file distributed along with this source code.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if WINDOWS
namespace MemoryModule
{
    using MemoryModule.Windows;
#else
namespace MemoryModule.Windows
{
#endif
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

            var handle = MemoryLoadLibraryEx(
                dataPtr,
                (ulong)length,
                allocMemory,
                freeMemory,
                loadLibrary,
                getProcAddress,
                freeLibrary,
                null
            );
            if (handle == null)
            {
                throw new Win32Exception((int)GetLastError());
            }
            return new IntPtr(handle);
        }

        /// <summary>
        /// Free previously loaded EXE/DLL.
        /// </summary>
        /// <param name="handle"></param>
        public static bool FreeLibrary(IntPtr handle)
        {
            return MemoryFreeLibrary((_MEMORYMODULE*)handle);
        }

        /// <summary>
        /// Gets a native function with the specified name,
        /// of the specified delegate type.
        /// </summary>
        /// <typeparam name="T">The delegate's type.</typeparam>
        /// <param name="name">The function's name</param>
        /// <returns></returns>
        public static T GetDelegate<T>(IntPtr module, string name) where T : Delegate
        {
            var namePtr = Marshal.StringToHGlobalAnsi(name);
            var funcPtr = (IntPtr)MemoryGetProcAddress(
                    (_MEMORYMODULE*)module,
                    (char *)namePtr
                );
            if (funcPtr == IntPtr.Zero)
            {
                throw new Win32Exception((int)GetLastError());
            }
            return (T)Marshal.GetDelegateForFunctionPointer(
                funcPtr,
                typeof(T)
            );
        }

        public static IntPtr GetSymbol(IntPtr module, string name)
        {
            char* namePtr = null;
            try
            {
                namePtr = (char *)Marshal.StringToHGlobalAnsi(name);
                return (IntPtr)MemoryGetProcAddress((_MEMORYMODULE*)module, namePtr);
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)namePtr);
            }
        }

        public static void* GetSymbolUnsafe(void* module, char* name)
        {
            return MemoryGetProcAddress((_MEMORYMODULE*)module, name);
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
            return VirtualAlloc(address, size, allocationType, protect);
        }

        internal static bool MemoryDefaultFree(void* lpAddress, UIntPtr dwSize, MemoryAllocation dwFreeType, void* userdata)
        {
            return VirtualFree(lpAddress, dwSize, dwFreeType);
        }

        internal static void* MemoryDefaultLoadLibrary(char* filename, void* userdata)
        {
            void* result;
            result = LoadLibraryA(filename);
            if (result == null)
            {
                return null;
            }

            return (void*)result;
        }

        internal static void* MemoryDefaultGetProcAddress(void* module, char* name, void* userdata)
        {
            return GetProcAddress(module, name);
        }

        internal static bool MemoryDefaultFreeLibrary(void* module, void* userdata)
        {
            return FreeLibrary(module);
        }
#endregion

        private static readonly uint HostMachine = ((Func<uint>)(() =>
        {
#if NET || NETSTANDARD || NET471_OR_GREATER
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    return Image.FileMachinei386;
                case Architecture.X64:
                    return Image.FileMachineAMD64;
                case Architecture.Arm:
                    return Image.FileMachineARMv7;
                case Architecture.Arm64:
                    return Image.FileMachineARM64;
                default:
                    throw new PlatformNotSupportedException();
            }
#else
            return (uint)(Environment.Is64BitProcess ? Image.FileMachineAMD64 : Image.FileMachinei386);
#endif
        }))();

        // Protection flags for memory pages (Executable, Readable, Writeable)
        private static readonly PageProtection[][][] ProtectionFlags = new PageProtection[][][]
        {
            new PageProtection[][]
            {
                // not executable
                new [] {PageProtection.NoAccess, PageProtection.WriteCopy},
                new [] {PageProtection.ReadOnly, PageProtection.ReadWrite},
            },
            new PageProtection[][]
            {
            // executable
                new [] { PageProtection.Execute, PageProtection.ExecuteWriteCopy},
                new [] { PageProtection.ExecuteRead, PageProtection.ExecuteReadWrite},
            },
        };

#region Core C functions
        private static void * MemoryLoadLibrary(void * data, ulong size)
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
            _MEMORYMODULE* result = null;
            IMAGE_DOS_HEADER* dos_header = null;
            _IMAGE_NT_HEADERS* old_header = null;
            byte* code = null; byte* headers = null;
            int locationDelta = 0;
            _SYSTEM_INFO sysInfo = default;
            _IMAGE_SECTION_HEADER* section;
            uint i = 0;
            ulong optionalSectionSize = 0;
            ulong lastSectionEnd = 0;
            ulong alignedImageSize = 0;

            try
            {
                #region 64-bit only
                POINTER_LIST* blockedMemory = null;
                #endregion

                if (!CheckSize(size, (ulong)sizeof(IMAGE_DOS_HEADER)))
                {
                    return null;
                }

                dos_header = (IMAGE_DOS_HEADER*)data;
                if (dos_header->e_magic != (ushort)Image.DosSignature)
                {
                    SetLastError(Error.BadExeFormat);
                    return null;
                }

                if (!CheckSize(size, (ulong)(dos_header->e_lfanew + sizeof(_IMAGE_NT_HEADERS))))
                {
                    return null;
                }

                old_header = (_IMAGE_NT_HEADERS*)&(((byte*)data)[dos_header->e_lfanew]);
                if (old_header->Signature != (uint)Image.NTSignature)
                {
                    SetLastError(Error.BadExeFormat);
                    return null;
                }

                if (old_header->FileHeader.Machine != HostMachine)
                {
                    SetLastError(Error.BadExeFormat);
                    return null;
                }

                if (Convert.ToBoolean(old_header->OptionalHeader.SectionAlignment & 1))
                {
                    // Only support section alignments that are a multiple of 2
                    SetLastError(Error.BadExeFormat);
                    return null;
                }

                section = IMAGE_FIRST_SECTION((UIntPtr)old_header);
                optionalSectionSize = old_header->OptionalHeader.SectionAlignment;
                for (i = 0; i < old_header->FileHeader.NumberOfSections; i++, section++)
                {
                    ulong endOfSection = 0;
                    if (section->SizeOfRawData == 0)
                    {
                        // Section without data in the DLL
                        endOfSection = section->VirtualAddress + optionalSectionSize;
                    }
                    else
                    {
                        endOfSection = section->VirtualAddress + section->SizeOfRawData;
                    }

                    if (endOfSection > lastSectionEnd)
                    {
                        lastSectionEnd = endOfSection;
                    }
                }

                GetNativeSystemInfo(&sysInfo);
                alignedImageSize = AlignValueUp(old_header->OptionalHeader.SizeOfImage, sysInfo.dwPageSize);
                if (alignedImageSize != AlignValueUp(lastSectionEnd, sysInfo.dwPageSize))
                {
                    SetLastError(Error.BadExeFormat);
                    return null;
                }

                // reserve memory for image of library
                // XXX: is it correct to commit the complete memory region at once?
                // calling DllEntry raises an exception if we don't...
                code = (byte*)allocMemory((void*)old_header->OptionalHeader.ImageBase,
                    (UIntPtr)alignedImageSize,
                    MemoryAllocation.Reserve | MemoryAllocation.Commit,
                    PageProtection.ReadWrite,
                    userdata);

                if (code == null)
                //            {
                {
                    // try to allocate memory at arbitrary position
                    code = (byte*)allocMemory(null,
                        (UIntPtr)alignedImageSize,
                        MemoryAllocation.Reserve | MemoryAllocation.Commit,
                        PageProtection.ReadWrite,
                        userdata);
                    if (code == null)
                    {
                        SetLastError(Error.OutOfMemory);
                        return null;
                    }
                }

                if (Environment.Is64BitProcess)
                {
                    //Memory block may not span 4 GB boundaries.
                    while ((((ulong)code) >> 32) < (((ulong)(code + alignedImageSize)) >> 32))
                    {
                        POINTER_LIST* node = (POINTER_LIST*)CStyleMemory.malloc((uint)sizeof(POINTER_LIST));
                        if (node == null)
                        {
                            freeMemory(code, (UIntPtr)0, MemoryAllocation.Release, userdata);
                            FreePointerList(blockedMemory, freeMemory, userdata);
                            SetLastError(Error.OutOfMemory);
                            return null;
                        }

                        node->next = blockedMemory;
                        node->address = code;
                        blockedMemory = node;

                        code = (byte*)allocMemory(null,
                                        (UIntPtr)alignedImageSize,
                                        MemoryAllocation.Reserve | MemoryAllocation.Commit,
                                        PageProtection.ReadWrite,
                                        userdata);
                        if (code == null)
                        {
                            FreePointerList(blockedMemory, freeMemory, userdata);
                            SetLastError(Error.OutOfMemory);
                            return null;
                        }
                    }
                }


                result = (_MEMORYMODULE*)HeapAlloc(GetProcessHeap(), (uint)Heap.ZeroMemory, (uint)Marshal.SizeOf(typeof(_MEMORYMODULE)));
                if (result == null)
                {
                    freeMemory(code, (UIntPtr)0, MemoryAllocation.Release, userdata);
                    if (Environment.Is64BitProcess)
                    {
                        FreePointerList(blockedMemory, freeMemory, userdata);
                    }
                    SetLastError(Error.OutOfMemory);
                    return null;
                }

                result->codeBase = code;
                result->isDLL = Convert.ToInt32((old_header->FileHeader.Characteristics & (uint)Image.FileDll) != 0);
                result->alloc = (void*)Marshal.GetFunctionPointerForDelegate(allocMemory);
                result->free = (void*)Marshal.GetFunctionPointerForDelegate(freeMemory);
                result->loadLibrary = (void*)Marshal.GetFunctionPointerForDelegate(loadLibrary);
                result->getProcAddress = (void*)Marshal.GetFunctionPointerForDelegate(getProcAddress);
                result->freeLibrary = (void*)Marshal.GetFunctionPointerForDelegate(freeLibrary);
                result->userdata = userdata;
                result->pageSize = sysInfo.dwPageSize;

                if (Environment.Is64BitProcess)
                {
                    result->blockedMemory = blockedMemory;
                }

                if (!CheckSize(size, old_header->OptionalHeader.SizeOfHeaders))
                {
                    goto error;
                }

                // commit memory for headers
                headers = (byte*)allocMemory(code,
                            (UIntPtr)old_header->OptionalHeader.SizeOfHeaders,
                            MemoryAllocation.Commit,
                            PageProtection.ReadWrite,
                            userdata);

                // copy PE header to code
                CStyleMemory.memcpy(headers, dos_header, old_header->OptionalHeader.SizeOfHeaders);

                result->headers = (_IMAGE_NT_HEADERS*)(&((byte*)(headers))[dos_header->e_lfanew]);

                // update position
                result->headers->OptionalHeader.ImageBase = (UIntPtr)code;

                // copy sections from DLL file block to new memory location
                if (!CopySections((byte*)data, size, old_header, result))
                {
                    goto error;
                }

                // adjust base address of imported data
                locationDelta = (int)(result->headers->OptionalHeader.ImageBase.ToUInt64() - old_header->OptionalHeader.ImageBase.ToUInt64());
                if (locationDelta != 0)
                {
                    result->isRelocated = Convert.ToInt32(PerformBaseRelocation(result, locationDelta));
                }
                else
                {
                    result->isRelocated = Convert.ToInt32(true);
                }

                // load required dlls and adjust function table of imports
                if (!BuildImportTable(result))
                {
                    goto error;
                }

                // mark memory pages depending on section headers and release
                // sections that are marked as "discardable"
                if (!FinalizeSections(result))
                {
                    goto error;
                }

                if (!InitTLS(result))
                {
                    goto error;
                }

                // TLS callbacks are executed BEFORE the main loading
                if (!ExecuteTLS(result))
                {
                    goto error;
                }

                // get entry point of loaded library
                if (result->headers->OptionalHeader.AddressOfEntryPoint != 0)
                {
                    if (Convert.ToBoolean(result->isDLL))
                    {
                        DllEntryProc DllEntry = (DllEntryProc)Marshal.GetDelegateForFunctionPointer(
                                (IntPtr)(code + result->headers->OptionalHeader.AddressOfEntryPoint),
                                typeof(DllEntryProc));
                        // notify library about attaching to process
                        bool successfull /*sic*/ = DllEntry(code, Dll.ProcessAttach, null);
                        if (!successfull)
                        {
                            SetLastError(Error.DllInitFailed);
                            goto error;
                        }
                        result->initialized = Convert.ToInt32(true);
                    }
                    else
                    {
                        result->exeEntry = code + result->headers->OptionalHeader.AddressOfEntryPoint;
                    }
                }
                else
                {
                    result->exeEntry = null;
                }

                return result;
            }
            catch
            {
                MemoryFreeLibrary(result);
                throw;
            }

            // Traditional error handling...
        error:
            // cleanup
            MemoryFreeLibrary(result);
            return null;
        }
        private static bool MemoryFreeLibrary(_MEMORYMODULE* mod)
        {
            _MEMORYMODULE* module = (_MEMORYMODULE*)mod;
            var managedModule = MEMORYMODULE.MarshalFrom(mod);

            if (module == null)
            {
                return true;
            }

            if (Convert.ToBoolean(module->initialized))
            {
                // notify library about detaching from process
                DllEntryProc DllEntry = (DllEntryProc)Marshal.GetDelegateForFunctionPointer(
                    (IntPtr)(module->codeBase + module->headers->OptionalHeader.AddressOfEntryPoint),
                    typeof(DllEntryProc)
                    );
                DllEntry(module->codeBase, Dll.ProcessDetach, null);

                // Detach the TLS else VirtualFree will fail.
                ExecuteTLS(module, Dll.ProcessDetach);
                FreeTLS(module);
            }

            // as nameExportsTable is implemented as a dictionary,
            // we must free it the managed way.
            if (module->nameExportsTable != IntPtr.Zero)
            {
                var obj = GCHandle.FromIntPtr(module->nameExportsTable);
                obj.Free();
            }

            if (module->modules != null)
            {
                // free previously opened libraries
                int i;
                for (i = 0; i < module->numModules; i++)
                {
                    if (module->modules[i] != null)
                    {
                        managedModule.freeLibrary(module->modules[i], module->userdata);
                    }
                }

                CStyleMemory.free(module->modules);
            }

            if (module->codeBase != null)
            {
                // release memory of library
                managedModule.free(module->codeBase, (UIntPtr)0, MemoryAllocation.Release, module->userdata);
            }

            if (Environment.Is64BitProcess)
            {
                FreePointerList(module->blockedMemory, module->free, module->userdata);
            }

            HeapFree(GetProcessHeap(), 0, module);

            return true;
        }

        private static void* MemoryGetProcAddress(_MEMORYMODULE* mod, char* name)
        {
            _MEMORYMODULE* module = (_MEMORYMODULE*)mod;
            byte* codeBase = module->codeBase;
            uint idx = 0;
            _IMAGE_EXPORT_DIRECTORY* exports = default;
            _IMAGE_DATA_DIRECTORY* directory = GET_HEADER_DICTIONARY(module, ImageDirectoryEntry.Export);
            if (directory->Size == 0)
            {
                // no export table found
                SetLastError(Error.ProcNotFound);
                return null;
            }

            exports = (_IMAGE_EXPORT_DIRECTORY*)(codeBase + directory->VirtualAddress);
            if (exports->NumberOfNames == 0 || exports->NumberOfFunctions == 0)
            {
                // DLL doesn't export anything
                SetLastError(Error.ProcNotFound);
                return null;
            }

            if (HIWORD((UIntPtr)name) == 0)
            {
                // load function by ordinal value
                if (LOWORD((UIntPtr)name) < exports->Base)
                {
                    SetLastError(Error.ProcNotFound);
                    return null;
                }

                idx = LOWORD((UIntPtr)name) - exports->Base;
            }
            else if (!Convert.ToBoolean(exports->NumberOfNames))
            {
                SetLastError(Error.ProcNotFound);
                return null;
            }
            else
            {
                Dictionary<string, uint> nameExports = null;

                // So I actually removed all of Thomas Heller's Binary search
                // implementation of MemoryGetProcAddress, and replace it with my
                // own managed Dictionary implementation.

                // Lazily build name table and sort it by names
                if (module->nameExportsTable == IntPtr.Zero)
                {
                    uint i = default;
                    uint* nameRef = (uint*)(codeBase + exports->AddressOfNames);
                    ushort* ordinal = (ushort*)(codeBase + exports->AddressOfNameOrdinals);
                    nameExports = new Dictionary<string, uint>();
                    var handle = GCHandle.Alloc(nameExports);
                    var entry = GCHandle.ToIntPtr(handle);
                    module->nameExportsTable = entry;

                    // Unlikely.
                    if (entry == IntPtr.Zero)
                    {
                        SetLastError(Error.OutOfMemory);
                        return null;
                    }

                    for (i = 0; i < exports->NumberOfNames; i++, nameRef++, ordinal++)
                    {
                        nameExports.Add(
                            Marshal.PtrToStringAnsi(
                                (IntPtr)((codeBase + (*nameRef)))
                                ),
                            *ordinal
                            );
                    }
                }
                else
                {
                    // We still have to get a reference to the
                    // dictionary.
                    var handle = GCHandle.FromIntPtr(module->nameExportsTable);
                    nameExports = handle.Target as Dictionary<string, uint>;
                }

                if (!nameExports.TryGetValue(Marshal.PtrToStringAnsi((IntPtr)name), out idx))
                {
                    SetLastError(Error.ProcNotFound);
                    return null;
                }
            }

            if (idx > exports->NumberOfFunctions)
            {
                // name <-> ordinal number don't match
                SetLastError(Error.ProcNotFound);
                return null;
            }

            // AddressOfFunctions contains the RVAs to the "real" functions
            return (void**)(codeBase + (*((uint*)(codeBase + exports->AddressOfFunctions + (idx * 4)))));
        }
#endregion

#region Helpers
        private static bool CheckSize(ulong size, ulong expected)
        {
            if (size < expected)
            {
                SetLastError(Error.InvalidData);
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FreePointerList(POINTER_LIST* head, void* freeMemoryPtr, void* userdata)
        {
            var freeMemory = Marshal.GetDelegateForFunctionPointer<CustomFreeFunc>((IntPtr)freeMemoryPtr);

            FreePointerList(head, freeMemory, userdata);
        }

        private static void FreePointerList(POINTER_LIST* head, CustomFreeFunc freeMemory, void* userdata)
        {
            POINTER_LIST* node = head;
            while (node != null)
            {
                POINTER_LIST* next;
                freeMemory(node->address, (UIntPtr)0, MemoryAllocation.Release, userdata);
                next = node->next;
                CStyleMemory.free(node);
                node = next;
            }
        }
#endregion

#region Functions that should have been macros
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static _IMAGE_SECTION_HEADER* IMAGE_FIRST_SECTION(UIntPtr ntheader)
        {
            return ((_IMAGE_SECTION_HEADER*)
                UIntPtr.Add(
                    UIntPtr.Add(ntheader, Marshal.OffsetOf(typeof(_IMAGE_NT_HEADERS), "OptionalHeader").ToInt32()),
                    ((_IMAGE_NT_HEADERS*)(ntheader))->FileHeader.SizeOfOptionalHeader)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static _IMAGE_DATA_DIRECTORY* GET_HEADER_DICTIONARY(_MEMORYMODULE* module, ImageDirectoryEntry idx)
        {
            // Hope this is safe?
            return &((_IMAGE_DATA_DIRECTORY*)&module->headers->OptionalHeader.DataDirectory)[(int)idx];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IMAGE_SNAP_BY_ORDINAL64(ulong Ordinal)
        {
            return (Ordinal & Image.OrdinalFlag64) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IMAGE_SNAP_BY_ORDINAL32(ulong Ordinal)
        {
            return (Ordinal & Image.OrdinalFlag32) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IMAGE_SNAP_BY_ORDINAL(UIntPtr ordinal)
        {
            if (Environment.Is64BitProcess)
            {
                return IMAGE_SNAP_BY_ORDINAL64(ordinal.ToUInt64());
            }
            else
            {
                return IMAGE_SNAP_BY_ORDINAL32(ordinal.ToUInt32());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong IMAGE_ORDINAL64(ulong Ordinal)
        {
            return Ordinal & 0xfffful;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint IMAGE_ORDINAL32(uint Ordinal)
        {
            return Ordinal & 0xffff;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UIntPtr IMAGE_ORDINAL(UIntPtr Ordinal)
        {
            if (Environment.Is64BitProcess)
            {
                return (UIntPtr)IMAGE_ORDINAL64(Ordinal.ToUInt64());
            }
            else
            {
                return (UIntPtr)IMAGE_ORDINAL32(Ordinal.ToUInt32());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ushort HIWORD(UIntPtr l)
        {
            // The original uses DWORD_PTR
            return (ushort)((((ulong)(l)) >> 16) & 0xffff);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ushort LOWORD(UIntPtr l)
        {
            // The original uses DWORD_PTR
            return ((ushort)(((ulong)(l)) & 0xffff));
        }
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
            return (byte*)data + offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static UIntPtr AlignValueDown(UIntPtr value, UIntPtr alignment)
        {
            return (UIntPtr)(value.ToUInt64() & ~(alignment.ToUInt64() - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void* AlignAddressDown(void* address, UIntPtr alignment)
        {
            return (void*)AlignValueDown((UIntPtr)address, alignment);
        }
#endregion

        static bool CopySections(byte* data, ulong size, _IMAGE_NT_HEADERS* old_headers, _MEMORYMODULE* modulePtr)
        {
            int i = default; int section_size = default;
            // To properly marshal all delegates.
            MEMORYMODULE module = MEMORYMODULE.MarshalFrom(modulePtr);
            byte* codeBase = module.codeBase;
            byte* dest;
            _IMAGE_SECTION_HEADER* section = IMAGE_FIRST_SECTION((UIntPtr)module.headers);
            for (i = 0; i < module.headers->FileHeader.NumberOfSections; i++, section++)
            {
                if (section->SizeOfRawData == 0)
                {
                    // section doesn't contain data in the dll itself, but may define
                    // uninitialized data
                    section_size = (int)old_headers->OptionalHeader.SectionAlignment;
                    if (section_size > 0)
                    {
                        dest = (byte*)module.alloc(codeBase + section->VirtualAddress,
                            (UIntPtr)section_size,
                            MemoryAllocation.Commit,
                            PageProtection.ReadWrite,
                            module.userdata
                            );

                        if (dest == null)
                        {
                            return false;
                        }

                        // Always use position from file to support alignments smaller
                        // than page size (allocation above will align to page size).
                        dest = codeBase + section->VirtualAddress;
                        // NOTE: On 64bit systems we truncate to 32bit here but expand
                        // again later when "PhysicalAddress" is used.
                        section->Misc.PhysicalAddress = (uint)(((UIntPtr)dest).ToUInt64() & 0xffffffff);
                        CStyleMemory.memset(dest, 0, (uint)section_size);
                    }

                    // section is empty
                    continue;
                }

                if (!CheckSize(size, section->PointerToRawData + section->SizeOfRawData))
                {
                    return false;
                }

                // commit memory block and copy data from dll
                dest = (byte*)module.alloc(codeBase + section->VirtualAddress,
                                            (UIntPtr)section->SizeOfRawData,
                                            MemoryAllocation.Commit,
                                            PageProtection.ReadWrite,
                                            module.userdata);
                if (dest == null)
                {
                    return false;
                }

                // Always use position from file to support alignments smaller
                // than page size (allocation above will align to page size).
                dest = codeBase + section->VirtualAddress;
                CStyleMemory.memcpy(dest, data + section->PointerToRawData, section->SizeOfRawData);
                // NOTE: On 64bit systems we truncate to 32bit here but expand
                // again later when "PhysicalAddress" is used.
                section->Misc.PhysicalAddress = (uint)(((UIntPtr)dest).ToUInt64() & 0xffffffff);
            }

            return true;
        }

        // In C#, subtracting 2 pointers gives a Int64 (or long).
        // So ptrdiff_t should be long.
        static bool PerformBaseRelocation(_MEMORYMODULE* module, long delta)
        {
            byte* codeBase = module->codeBase;
            _IMAGE_BASE_RELOCATION* relocation = default;
            _IMAGE_DATA_DIRECTORY* directory = GET_HEADER_DICTIONARY(module, ImageDirectoryEntry.BaseReloc);
            if (directory->Size == 0)
            {
                return delta == 0;
            }

            relocation = (_IMAGE_BASE_RELOCATION*)(codeBase + directory->VirtualAddress);
            for (; relocation->VirtualAddress > 0;)
            {
                uint i = default;
                byte* dest = codeBase + relocation->VirtualAddress;
                ushort* relInfo = (ushort*)OffsetPointer(relocation, Image.SizeOfBaseRelocation);
                for (i = 0; i < ((relocation->SizeOfBlock - Image.SizeOfBaseRelocation) / 2); i++, relInfo++)
                {
                    // the upper 4 bits define the type of relocation
                    int type = *relInfo >> 12;
                    // the lower 12 bits define the offset
                    int offset = *relInfo & 0xfff;

                    switch ((ImageRelocation)type)
                    {
                        case ImageRelocation.BasedAbsolute:
                            // skip relocation
                            break;

                        case ImageRelocation.BasedHighLow:
                            // change complete 32 bit address
                            {
                                uint* patchAddrHL = (uint*)(dest + offset);
                                *patchAddrHL += (uint)delta;
                            }
                            break;
                        case ImageRelocation.BasedDir64:
                            if (Environment.Is64BitProcess)
                            {
                                long* patchAddr64 = (long*)(dest + offset);
                                *patchAddr64 += (long)delta;
                            }
                            else
                            {
                                Console.WriteLine($"Unknown 64-bit relocation in a 32-bit process");
                            }
                            break;

                        default:
                            Console.WriteLine($"Unknown relocation: {(ImageRelocation)type}");
                            break;
                    }
                }

                // advance to next relocation block
                relocation = (_IMAGE_BASE_RELOCATION*)OffsetPointer(relocation, (int)relocation->SizeOfBlock);
            }
            return true;
        }

        static bool BuildImportTable(_MEMORYMODULE* module)
        {
            var managedModule = MEMORYMODULE.MarshalFrom(module);
            byte* codeBase = module->codeBase;
            _IMAGE_IMPORT_DESCRIPTOR* importDesc = default;
            bool result = true;

            _IMAGE_DATA_DIRECTORY* directory = GET_HEADER_DICTIONARY(module, ImageDirectoryEntry.Import);
            if (directory->Size == 0)
            {
                return true;
            }

            importDesc = (_IMAGE_IMPORT_DESCRIPTOR*)(codeBase + directory->VirtualAddress);
            for (; !Convert.ToBoolean(IsBadReadPtr(importDesc, (uint)sizeof(_IMAGE_IMPORT_DESCRIPTOR))) && importDesc->Name != 0; importDesc++)
            {
                UIntPtr* thunkRef = default;
                void** funcRef = default;
                void** tmp = default;
                char* moduleName = (char*)(codeBase + importDesc->Name);
                void* handle = managedModule.loadLibrary(moduleName, module->userdata);
                if (handle == null)
                {
                    throw new NativeAssemblyLoadException($"Cannot load dependency: {Marshal.PtrToStringAnsi((IntPtr)moduleName)}");
                }

                tmp = (void**)CStyleMemory.realloc(module->modules, (uint)((module->numModules + 1) * sizeof(void*)));
                if (tmp == null)
                {
                    managedModule.freeLibrary(handle, module->userdata);
                    SetLastError(Error.OutOfMemory);
                    result = false;
                    break;
                }
                module->modules = tmp;

                module->modules[module->numModules++] = handle;
                if (importDesc->OriginalFirstThunk != 0)
                {
                    thunkRef = (UIntPtr*)(codeBase + importDesc->OriginalFirstThunk);
                    funcRef = (void**)(codeBase + importDesc->FirstThunk);
                }
                else
                {
                    // no hint table
                    thunkRef = (UIntPtr*)(codeBase + importDesc->FirstThunk);
                    funcRef = (void**)(codeBase + importDesc->FirstThunk);
                }
                for (; *thunkRef != UIntPtr.Zero; thunkRef++, funcRef++)
                {
                    if (IMAGE_SNAP_BY_ORDINAL(*thunkRef))
                    {
                        *funcRef = managedModule.getProcAddress(handle, (char*)IMAGE_ORDINAL(*thunkRef), module->userdata);
                    }
                    else
                    {
                        _IMAGE_IMPORT_BY_NAME* thunkData = (_IMAGE_IMPORT_BY_NAME*)(codeBase + (*thunkRef).ToUInt64());
                        *funcRef = managedModule.getProcAddress(handle, (char*)thunkData->Name, module->userdata);
                    }
                    if (*funcRef == null)
                    {
                        result = false;
                        break;
                    }
                }

                if (!result)
                {
                    managedModule.freeLibrary(handle, module->userdata);
                    SetLastError(Error.ProcNotFound);
                    break;
                }
            }

            return result;
        }

        static UIntPtr GetRealSectionSize(_MEMORYMODULE* module, _IMAGE_SECTION_HEADER* section)
        {
            uint size = section->SizeOfRawData;
            if (size == 0)
            {
                if (Convert.ToBoolean(section->Characteristics & Image.SectionCntInitializedData))
                {
                    size = module->headers->OptionalHeader.SizeOfInitializedData;
                }
                else if (Convert.ToBoolean(section->Characteristics & Image.SectionCntUninitializedData))
                {
                    size = module->headers->OptionalHeader.SizeOfUninitializedData;
                }
            }
            return (UIntPtr)size;
        }

        static bool FinalizeSection(_MEMORYMODULE* module, SECTIONFINALIZEDATA* sectionData)
        {
            var managedModule = MEMORYMODULE.MarshalFrom(module);
            PageProtection protect = default; uint oldProtect = default;
            bool executable = default;
            bool readable = default;
            bool writeable = default;

            if (sectionData->size == 0)
            {
                return true;
            }

            if (Convert.ToBoolean(sectionData->characteristics & (uint)ImageSectionMemory.Discardable))
            {
                // section is not needed any more and can safely be freed
                if (sectionData->address == sectionData->alignedAddress &&
                    (sectionData->last ||
                    module->headers->OptionalHeader.SectionAlignment == module->pageSize ||
                    (sectionData->size % module->pageSize) == 0)
                    )
                {
                    // Only allowed to decommit whole pages
                    managedModule.free(sectionData->address, (UIntPtr)sectionData->size, MemoryAllocation.Decommit, module->userdata);
                }
                return true;
            }

            // determine protection flags based on characteristics
            executable = (sectionData->characteristics & (uint)ImageSectionMemory.Exectute) != 0;
            readable = (sectionData->characteristics & (uint)ImageSectionMemory.Read) != 0;
            writeable = (sectionData->characteristics & (uint)ImageSectionMemory.Write) != 0;
            protect = ProtectionFlags[Convert.ToInt32(executable)][Convert.ToInt32(readable)][Convert.ToInt32(writeable)];
            if (Convert.ToBoolean(sectionData->characteristics & (uint)ImageSectionMemory.NotCached))
            {
                protect |= PageProtection.PAGE_NOCACHE;
            }

            // change memory access flags
            if (VirtualProtect(sectionData->address, (UIntPtr)sectionData->size, protect, &oldProtect) == 0)
            {
                Console.Error.WriteLine("Error protecting memory page");
                return false;
            }

            return true;
        }

        static bool FinalizeSections(_MEMORYMODULE* module)
        {
            int i;
            _IMAGE_SECTION_HEADER* section = IMAGE_FIRST_SECTION((UIntPtr)module->headers);

            UIntPtr imageOffset;
            if (Environment.Is64BitProcess)
            {
                // "PhysicalAddress" might have been truncated to 32bit above, expand to
                // 64bits again.
                imageOffset = (UIntPtr)(((UIntPtr)(module->headers->OptionalHeader.ImageBase)).ToUInt64() & 0xffffffff00000000);
            }
            else
            {
                imageOffset = UIntPtr.Zero;
            }

            SECTIONFINALIZEDATA sectionData = default;
            sectionData.address = (void*)(((UIntPtr)section->Misc.PhysicalAddress).ToUInt64() | imageOffset.ToUInt64());
            sectionData.alignedAddress = AlignAddressDown(sectionData.address, (UIntPtr)module->pageSize);
            sectionData.size = (uint)GetRealSectionSize(module, section);
            sectionData.characteristics = section->Characteristics;
            sectionData.last = false;
            section++;

            // loop through all sections and change access flags
            for (i = 1; i < module->headers->FileHeader.NumberOfSections; ++i, ++section)
            {
                void* sectionAddress = (void*)(((UIntPtr)section->Misc.PhysicalAddress).ToUInt64() | imageOffset.ToUInt64());
                void* alignedAddress = AlignAddressDown(sectionAddress, (UIntPtr)module->pageSize);
                UIntPtr sectionSize = GetRealSectionSize(module, section);
                // Combine access flags of all sections that share a page
                // TODO(fancycode): We currently share flags of a trailing large section
                //   with the page of a first small section. This should be optimized.
                if (sectionData.alignedAddress == alignedAddress
                    || UIntPtr.Add((UIntPtr)sectionData.address, (int)sectionData.size).ToUInt64() > ((UIntPtr)alignedAddress).ToUInt64())
                {
                    // Section shares page with previous
                    if ((section->Characteristics & (uint)ImageSectionMemory.Discardable) == 0
                        || (sectionData.characteristics & (uint)ImageSectionMemory.Discardable) == 0)
                    {
                        sectionData.characteristics = (sectionData.characteristics | section->Characteristics) & (~(uint)ImageSectionMemory.Discardable);
                    }
                    else
                    {
                        sectionData.characteristics |= section->Characteristics;
                    }
                    sectionData.size = (uint)(((UIntPtr)(sectionAddress)).ToUInt64()
                        + ((UIntPtr)sectionSize).ToUInt64()
                        - ((UIntPtr)sectionData.address).ToUInt64());
                    continue;
                }

                if (!FinalizeSection(module, &sectionData))
                {
                    return false;
                }
                sectionData.address = sectionAddress;
                sectionData.alignedAddress = alignedAddress;
                sectionData.size = (uint)sectionSize;
                sectionData.characteristics = section->Characteristics;
            }
            sectionData.last = true;
            if (!FinalizeSection(module, &sectionData))
            {
                return false;
            }
            return true;
        }

        static bool InitTLS(_MEMORYMODULE* module)
        {
            byte* codeBase = module->codeBase;
            _IMAGE_TLS_DIRECTORY* tls = default;
            void** callback = default;

            _IMAGE_DATA_DIRECTORY* directory = GET_HEADER_DICTIONARY(module, ImageDirectoryEntry.Tls);
            if (directory->VirtualAddress == 0)
            {
                return true;
            }

            tls = (_IMAGE_TLS_DIRECTORY*)(codeBase + directory->VirtualAddress);

            // Assign this library an unique index
            uint newIndex = TlsAlloc();

            if (newIndex == TLS_OUT_OF_INDEXES)
            {
                return false;
            }

            *(uint*)tls->AddressOfIndex = newIndex;
            return true;
        }

        static bool ExecuteTLS(_MEMORYMODULE* module, Dll status = Dll.ProcessAttach)
        {
            byte* codeBase = module->codeBase;
            _IMAGE_TLS_DIRECTORY* tls = default;
            void** callback = default;

            _IMAGE_DATA_DIRECTORY* directory = GET_HEADER_DICTIONARY(module, ImageDirectoryEntry.Tls);
            if (directory->VirtualAddress == 0)
            {
                return true;
            }

            tls = (_IMAGE_TLS_DIRECTORY*)(codeBase + directory->VirtualAddress);
            callback = (void**)tls->AddressOfCallBacks;
            if (callback != null)
            {
                while (*callback != null)
                {
                    var callbackFunc = (PIMAGE_TLS_CALLBACK)Marshal.GetDelegateForFunctionPointer(
                        (IntPtr)(*callback), typeof(PIMAGE_TLS_CALLBACK));
                    callbackFunc(codeBase, (uint)status, null);
                    callback++;
                }
            }
            return true;
        }

        static bool FreeTLS(_MEMORYMODULE* module)
        {
            byte* codeBase = module->codeBase;
            _IMAGE_TLS_DIRECTORY* tls = default;
            void** callback = default;

            _IMAGE_DATA_DIRECTORY* directory = GET_HEADER_DICTIONARY(module, ImageDirectoryEntry.Tls);
            if (directory->VirtualAddress == 0)
            {
                return true;
            }

            tls = (_IMAGE_TLS_DIRECTORY*)(codeBase + directory->VirtualAddress);

            return TlsFree(*(uint*)tls->AddressOfIndex);
        }

        #region Nightmare
        [StructLayout(LayoutKind.Explicit, Size=4, Pack=1)]
        struct __anontype_1639d593_0026
        {
            [FieldOffset(0)]
            public uint dwOemID;
            [FieldOffset(0)]
            public __anontype_1639d593_0027 s;
        }

        [StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
        struct __anontype_1639d593_0027
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
        }

        [StructLayout(LayoutKind.Explicit, Size=4, Pack=1)]
        struct __anontype_1639d593_0036
        {
            [FieldOffset(0)]
            public uint PhysicalAddress;
            [FieldOffset(0)]
            public uint VirtualSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack=4)]
        class MEMORYMODULE
        {
            public unsafe _IMAGE_NT_HEADERS* headers;
            public unsafe byte* codeBase;
            public unsafe void** modules;
            public int numModules;
            [MarshalAs(UnmanagedType.Bool)]
            public bool initialized;
            [MarshalAs(UnmanagedType.Bool)]
            public bool isDLL;
            [MarshalAs(UnmanagedType.Bool)]
            public bool isRelocated;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public CustomAllocFunc alloc;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public CustomFreeFunc free;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public CustomLoadLibraryFunc loadLibrary;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public CustomGetProcAddressFunc getProcAddress;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public CustomFreeLibraryFunc freeLibrary;
            public IntPtr nameExportsTable;
            public unsafe void* userdata;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public unsafe ExeEntryProc exeEntry;
            public uint pageSize;
            public unsafe POINTER_LIST* blockedMemory;

            public static MEMORYMODULE MarshalFrom(_MEMORYMODULE* ptr)
            {
                MEMORYMODULE module = new MEMORYMODULE();
                Marshal.PtrToStructure((IntPtr)ptr, module);
                return module;
            }

            public void MarshalTo(_MEMORYMODULE* ptr)
            {
                Marshal.StructureToPtr(this, (IntPtr)ptr, true);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct _MEMORYMODULE
        {
            public unsafe _IMAGE_NT_HEADERS* headers;
            public unsafe byte* codeBase;
            public unsafe void** modules;
            public int numModules;
            public int initialized;
            public int isDLL;
            public int isRelocated;
            public void* alloc;
            public void* free;
            public void* loadLibrary;
            public void* getProcAddress;
            public void* freeLibrary;
            public IntPtr nameExportsTable;
            public unsafe void* userdata;
            public unsafe void* exeEntry;
            public uint pageSize;
            public unsafe POINTER_LIST* blockedMemory;
        }

        [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 4)]
        public struct POINTER_LIST
        {
            public unsafe POINTER_LIST* next;
            public unsafe void* address;
        }

        [StructLayout(LayoutKind.Sequential, Size=20, Pack=4)]
        struct SECTIONFINALIZEDATA
        {
            public unsafe void* address;
            public unsafe void* alignedAddress;
            public uint size;
            public uint characteristics;
            public bool last;
        }

        [StructLayout(LayoutKind.Explicit, Size=4, Pack=4)]
        struct __anontype_b18cdc1e_0003
        {
            [FieldOffset(0)]
            public __anontype_b18cdc1e_0004 DUMMYSTRUCTNAME;
            [FieldOffset(0)]
            public uint Name;
            [FieldOffset(0)]
            public ushort Id;
        }

        [StructLayout(LayoutKind.Sequential, Size=4, Pack=4)]
        struct __anontype_b18cdc1e_0004
        {
            public uint bits0;
        }

        [StructLayout(LayoutKind.Explicit, Size=4, Pack=4)]
        struct __anontype_b18cdc1e_0005
        {
            [FieldOffset(0)]
            public uint OffsetToData;
            [FieldOffset(0)]
            public __anontype_b18cdc1e_0006 DUMMYSTRUCTNAME2;
        }

        [StructLayout(LayoutKind.Sequential, Size=4, Pack=4)]
        struct __anontype_b18cdc1e_0006
        {
            public uint bits0;
        }

        [StructLayout(LayoutKind.Sequential, Size=8, Pack=4)]
        struct _IMAGE_BASE_RELOCATION
        {
            public uint VirtualAddress;
            public uint SizeOfBlock;
        }

        [StructLayout(LayoutKind.Sequential, Size=8, Pack=1)]
        struct _IMAGE_DATA_DIRECTORY
        {
            public uint VirtualAddress;
            public uint Size;
        }

        [StructLayout(LayoutKind.Sequential, Size=0x80, Pack=1)]
        struct _IMAGE_DATA_DIRECTORY_array_128
        {
            public _IMAGE_DATA_DIRECTORY this[int offset]
            {
                get
                {
                    fixed (void* voidArrayBase = &this)
                    {
                        _IMAGE_DATA_DIRECTORY* arrayBase = (_IMAGE_DATA_DIRECTORY*)voidArrayBase;
                        return arrayBase[offset];
                    }
                }
                set
                {
                    fixed (void* voidArrayBase = &this)
                    {
                        _IMAGE_DATA_DIRECTORY* arrayBase = (_IMAGE_DATA_DIRECTORY*)voidArrayBase;
                        arrayBase[offset] = value;
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Size=40, Pack=1)]
        struct _IMAGE_EXPORT_DIRECTORY
        {
            public uint Characteristics;
            public uint TimeDateStamp;
            public ushort MajorVersion;
            public ushort MinorVersion;
            public uint Name;
            public uint Base;
            public uint NumberOfFunctions;
            public uint NumberOfNames;
            public uint AddressOfFunctions;
            public uint AddressOfNames;
            public uint AddressOfNameOrdinals;
        }

        [StructLayout(LayoutKind.Sequential, Size=20, Pack=1)]
        struct _IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        }

        [StructLayout(LayoutKind.Sequential, Size=4, Pack=2)]
        struct _IMAGE_IMPORT_BY_NAME
        {
            public ushort Hint;
            public fixed byte Name[1];
        }

        [StructLayout(LayoutKind.Explicit, Size=20, Pack=1)]
        struct _IMAGE_IMPORT_DESCRIPTOR
        {
            [FieldOffset(0)]
            public uint Characteristics;
            [FieldOffset(0)]
            public uint OriginalFirstThunk;
            [FieldOffset(sizeof(uint))]
            public uint TimeDateStamp;
            [FieldOffset(sizeof(uint) * 2)]
            public uint ForwarderChain;
            [FieldOffset(sizeof(uint) * 3)]
            public uint Name;
            [FieldOffset(sizeof(uint) * 4)]
            public uint FirstThunk;
        }

        [StructLayout(LayoutKind.Sequential, Size=0xf8, Pack=1)]
        struct _IMAGE_NT_HEADERS
        {
            public uint Signature;
            public _IMAGE_FILE_HEADER FileHeader;
            public _IMAGE_OPTIONAL_HEADER OptionalHeader;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        struct _IMAGE_OPTIONAL_HEADER
        {
            const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;

            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;

            // In C#, we don't need macros.
            // We have more evil stuff.
            [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
            private struct DUMMY8BYTEVALUE
            {
                // 32 bit fields
                [FieldOffset(0)]
                public uint BaseOfData;
                [FieldOffset(4)]
                public uint ImageBase32;
                // 64 bit fields
                [FieldOffset(0)]
                public ulong ImageBase64;
            }
            private DUMMY8BYTEVALUE _architectureSpecificValue1;

            public uint BaseOfData
            {
                get => _architectureSpecificValue1.BaseOfData;
                set => _architectureSpecificValue1.BaseOfData = value;
            }
            public UIntPtr ImageBase
            {
                get => Environment.Is64BitProcess ?
                    (UIntPtr)_architectureSpecificValue1.ImageBase64 :
                    (UIntPtr)_architectureSpecificValue1.ImageBase32;
                set
                {
                    if (Environment.Is64BitProcess)
                    {
                        _architectureSpecificValue1.ImageBase64 = (ulong)value;
                    }
                    else
                    {
                        _architectureSpecificValue1.ImageBase32 = (uint)value;
                    }
                }
            }

            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public UIntPtr SizeOfStackReserve;
            public UIntPtr SizeOfStackCommit;
            public UIntPtr SizeOfHeapReserve;
            public UIntPtr SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            public _IMAGE_DATA_DIRECTORY_array_128 DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential, Size=0x10, Pack=4)]
        struct _IMAGE_RESOURCE_DATA_ENTRY
        {
            public uint OffsetToData;
            public uint Size;
            public uint CodePage;
            public uint Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Size=4, Pack=2)]
        struct _IMAGE_RESOURCE_DIR_STRING_U
        {
            public ushort Length;
            public fixed char NameString[1];
        }

        [StructLayout(LayoutKind.Sequential, Size=0x10, Pack=4)]
        struct _IMAGE_RESOURCE_DIRECTORY
        {
            public uint Characteristics;
            public uint TimeDateStamp;
            public ushort MajorVersion;
            public ushort MinorVersion;
            public ushort NumberOfNamedEntries;
            public ushort NumberOfIdEntries;
        }

        [StructLayout(LayoutKind.Sequential, Size=8, Pack=4)]
        struct _IMAGE_RESOURCE_DIRECTORY_ENTRY
        {
            public __anontype_b18cdc1e_0003 DUMMYUNIONNAME;
            public __anontype_b18cdc1e_0005 DUMMYUNIONNAME2;
        }

        [StructLayout(LayoutKind.Sequential, Size=40, Pack=1)]
        struct _IMAGE_SECTION_HEADER
        {
            public fixed byte Name[8];
            public __anontype_1639d593_0036 Misc;
            public uint VirtualAddress;
            public uint SizeOfRawData;
            public uint PointerToRawData;
            public uint PointerToRelocations;
            public uint PointerToLinenumbers;
            public ushort NumberOfRelocations;
            public ushort NumberOfLinenumbers;
            public uint Characteristics;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct _IMAGE_TLS_DIRECTORY
        {
            /// <summary>
            /// Uses an UIntPtr to fuck Architecture-specific types.
            /// </summary>
            public UIntPtr StartAddressOfRawData;
            public UIntPtr EndAddressOfRawData;
            public UIntPtr AddressOfIndex;
            public UIntPtr AddressOfCallBacks;
            public uint SizeOfZeroFill;
            public uint Characteristics;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        struct _SYSTEM_INFO
        {
            public __anontype_1639d593_0026 u;
            public uint dwPageSize;
            public unsafe void* lpMinimumApplicationAddress;
            public unsafe void* lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [StructLayout(LayoutKind.Sequential, Size=0x40, Pack=1)]
        struct IMAGE_DOS_HEADER
        {
            public ushort e_magic;
            public ushort e_cblp;
            public ushort e_cp;
            public ushort e_crlc;
            public ushort e_cparhdr;
            public ushort e_minalloc;
            public ushort e_maxalloc;
            public ushort e_ss;
            public ushort e_sp;
            public ushort e_csum;
            public ushort e_ip;
            public ushort e_cs;
            public ushort e_lfarlc;
            public ushort e_ovno;
            public fixed ushort e_res[4];
            public ushort e_oemid;
            public ushort e_oeminfo;
            public fixed ushort e_res2[10];
            public int e_lfanew;
        }
#endregion

#region Delegates
        //FARPROC:
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate IntPtr FARPROC();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void PIMAGE_TLS_CALLBACK(void* DllHandle, uint Reason, void* Reserved);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate bool DllEntryProc(void* hinstDLL, Dll fdwReason, void* lpReserved);
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate int ExeEntryProc();
        #endregion

        #region P/Invoke
        const uint TLS_OUT_OF_INDEXES = 0xFFFFFFFF;

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetLastError(Error dwErrCode);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern uint GetLastError();

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe void GetNativeSystemInfo(void* lpSystemInfo);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe void* HeapAlloc(void* hHeap, uint dwFlags, uint dwBytes);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int HeapFree(void* hHeap, uint dwFlags, void* lpMem);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe void* GetProcessHeap();

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int IsBadReadPtr(void* lp, uint ucb);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe void* VirtualAlloc(void* lpAddress, UIntPtr dwSize, MemoryAllocation flAllocationType, PageProtection flProtect);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe bool VirtualFree(void* lpAddress, UIntPtr dwSize, MemoryAllocation dwFreeType);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        // The library name seems to always be ANSI, so we must use LoadLibraryA in this case.
        private static extern unsafe void* LoadLibraryA(char* lpLibFileName);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe void* GetProcAddress(void* hModule, char* lpProcName);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe bool FreeLibrary(void* hLibModule);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int VirtualProtect(void* lpAddress, UIntPtr dwSize, PageProtection flNewProtect, void* lpflOldProtect);

        // Who says Windows users have less freedom than Linux?
        // F**k you, glibc.
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern unsafe uint TlsAlloc();

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern unsafe bool TlsFree(uint index);
        #endregion
    }
}

