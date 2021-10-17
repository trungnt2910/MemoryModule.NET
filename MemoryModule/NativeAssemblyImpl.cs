using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule
{
    /// <summary>
    /// Implementation abstraction for Windows and Linux
    /// </summary>
    internal static unsafe class NativeAssemblyImpl
    {
        #region Delegates
        public delegate IntPtr PlatformLoadLibraryFunc(
            byte* dataPtr,
            long length,
            CustomAllocFunc allocMemory = null,
            CustomFreeFunc freeMemory = null,
            CustomLoadLibraryFunc loadLibrary = null,
            CustomGetProcAddressFunc getProcAddress = null,
            CustomFreeLibraryFunc freeLibrary = null);

        public delegate bool PlatformFreeLibraryFunc(IntPtr handle);

        public delegate IntPtr PlatformGetSymbolFunc(IntPtr module, string name);

        internal delegate void* PlatformGetSymbolUnsafeFunc(void* module, char* name);
        #endregion

        public static readonly PlatformLoadLibraryFunc LoadLibrary;
        public static readonly PlatformFreeLibraryFunc FreeLibrary;
        public static readonly PlatformGetSymbolFunc GetSymbol;

        public static readonly CustomLoadLibraryFunc MemoryDefaultLoadLibrary;
        public static readonly CustomFreeLibraryFunc MemoryDefaultFreeLibrary;
        public static readonly CustomGetProcAddressFunc MemoryDefaultGetProcAddress;

        internal static readonly PlatformGetSymbolUnsafeFunc GetSymbolUnsafe;

        static NativeAssemblyImpl()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LoadLibrary = Windows.NativeAssemblyImpl.LoadLibrary;
                FreeLibrary = Windows.NativeAssemblyImpl.FreeLibrary;
                GetSymbol = Windows.NativeAssemblyImpl.GetSymbol;

                MemoryDefaultLoadLibrary = Windows.NativeAssemblyImpl.MemoryDefaultLoadLibrary;
                MemoryDefaultFreeLibrary = Windows.NativeAssemblyImpl.MemoryDefaultFreeLibrary;
                MemoryDefaultGetProcAddress = Windows.NativeAssemblyImpl.MemoryDefaultGetProcAddress;

                GetSymbolUnsafe = Windows.NativeAssemblyImpl.GetSymbolUnsafe;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                LoadLibrary = Linux.NativeAssemblyImpl.LoadLibrary;
                FreeLibrary = Linux.NativeAssemblyImpl.FreeLibrary;
                GetSymbol = Linux.NativeAssemblyImpl.GetSymbol;

                MemoryDefaultLoadLibrary = Linux.NativeAssemblyImpl.MemoryDefaultLoadLibrary;
                MemoryDefaultFreeLibrary = Linux.NativeAssemblyImpl.MemoryDefaultFreeLibrary;
                MemoryDefaultGetProcAddress = Linux.NativeAssemblyImpl.MemoryDefaultGetProcAddress;

                GetSymbolUnsafe = Linux.NativeAssemblyImpl.GetSymbolUnsafe;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                LoadLibrary = MacOS.NativeAssemblyImpl.LoadLibrary;
                FreeLibrary = MacOS.NativeAssemblyImpl.FreeLibrary;
                GetSymbol = MacOS.NativeAssemblyImpl.GetSymbol;

                MemoryDefaultLoadLibrary = MacOS.NativeAssemblyImpl.MemoryDefaultLoadLibrary;
                MemoryDefaultFreeLibrary = MacOS.NativeAssemblyImpl.MemoryDefaultFreeLibrary;
                MemoryDefaultGetProcAddress = MacOS.NativeAssemblyImpl.MemoryDefaultGetProcAddress;

                GetSymbolUnsafe = MacOS.NativeAssemblyImpl.GetSymbolUnsafe;
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported platform! Please open an issue!");
            }
        }
    }
}
