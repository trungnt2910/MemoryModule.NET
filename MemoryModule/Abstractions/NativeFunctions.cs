using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Abstractions
{
    /// <summary>
    /// A base class for platform-specific native functions.
    /// </summary>
    public abstract class NativeFunctions : INativeFunctions
    {
        public abstract bool FreeLibrary(IntPtr handle);
        public abstract IntPtr GetSymbolFromLibrary(IntPtr handle, string name);
        public abstract IntPtr GetSymbolFromLibrary(IntPtr handle, IntPtr nameValue);
        public abstract IntPtr LoadLibrary(string name);
        public abstract IntPtr VirtualAllocate(IntPtr hint, ulong size, MemoryProtection protection);
        public abstract bool VirtualFree(IntPtr addr, ulong size);
        public abstract bool VirtualProtect(IntPtr addr, ulong size, MemoryProtection protection);

#if STANDALONE
        public abstract IntPtr CopyMemory(IntPtr dest, IntPtr src, ulong size);
        public abstract IntPtr FillMemory(IntPtr dest, byte ch, ulong size);
#else
        public IntPtr CopyMemory(IntPtr dest, IntPtr src, ulong size)
        {
            unsafe
            {
                Unsafe.CopyBlockUnaligned((void*)dest, (void*)src, (uint)size);
            }
            return dest;
        }

        public IntPtr FillMemory(IntPtr dest, byte ch, ulong size)
        {
            unsafe
            {
                Unsafe.InitBlockUnaligned((void*)dest, ch, (uint)size);
            }
            return dest;
        }
#endif

        /// <summary>
        /// Gets the default set of native functions for the current platform.
        /// </summary>
        public static NativeFunctions Default { get; private set; }

        static NativeFunctions()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Default = new Windows.WindowsNativeFunctions();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Default = new Linux.LinuxNativeFunctions();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Default = new MacOS.MacNativeFunctions();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}
