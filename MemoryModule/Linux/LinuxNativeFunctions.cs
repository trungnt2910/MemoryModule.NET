using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Linux
{
    class LinuxNativeFunctions : NativeFunctions
    {
        public override bool FreeLibrary(IntPtr handle)
        {
            return dlclose(handle) == 0;
        }

        public override IntPtr GetSymbolFromLibrary(IntPtr handle, string name)
        {
            if (handle == IntPtr.Zero)
            {
                return dlsym(IntPtr.Zero /* RTLD_DFEAULT */, name);
            }
            return dlsym(handle, name);
        }

        public override IntPtr GetSymbolFromLibrary(IntPtr handle, IntPtr nameValue)
        {
            if (handle == IntPtr.Zero)
            {
                return dlsym(IntPtr.Zero /* RTLD_DFEAULT */, nameValue);
            }
            return dlsym(handle, nameValue);
        }

        public override IntPtr LoadLibrary(string name)
        {
            return dlopen(name);
        }

        public override IntPtr VirtualAllocate(IntPtr hint, ulong size, MemoryProtection protection)
        {
            return mmap(hint, (UIntPtr)size, ConvertProtectionFlags(protection), MmapMappingFlags.Private | MmapMappingFlags.Anonymous);
        }

        public override bool VirtualFree(IntPtr addr, ulong size)
        {
            return munmap(addr, (UIntPtr)size) == 0;
        }

        public override bool VirtualProtect(IntPtr addr, ulong size, MemoryProtection protection)
        {
            return mprotect(addr, (UIntPtr)size, ConvertProtectionFlags(protection)) == 0;
        }

        private MmapProtectionFlags ConvertProtectionFlags(MemoryProtection protection)
        {
            var result = (MmapProtectionFlags)0;
            if (protection.HasFlag(MemoryProtection.Read))
            {
                result |= MmapProtectionFlags.Read;
            }
            if (protection.HasFlag(MemoryProtection.Write))
            {
                result |= MmapProtectionFlags.Write;
            }
            if (protection.HasFlag(MemoryProtection.Execute))
            {
                result |= MmapProtectionFlags.Execute;
            }
            return result;
        }


        [DllImport("libc", SetLastError = true)]
        private static extern IntPtr mmap(
            IntPtr addr,
            UIntPtr length,
            MmapProtectionFlags protectionFlags,
            MmapMappingFlags mappingFlags,
            int fileDescriptor = -1, int offset = 0);

        [DllImport("libc", SetLastError = true)]
        private static extern int mprotect(IntPtr addr, UIntPtr length, MmapProtectionFlags protectionFlags);

        [DllImport("libc", SetLastError = true)]
        private static extern int munmap(IntPtr addr, UIntPtr length);

        [DllImport("dl", SetLastError = true)]
        private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string name, int mode = 0x01 | 0x00100 /*RTLD_LAZY | RTLD_GLOBAL*/);

        [DllImport("dl", SetLastError = true)]
        private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("dl", SetLastError = true)]
        private static extern IntPtr dlsym(IntPtr handle, IntPtr nameValue);

        [DllImport("dl", SetLastError = true)]
        private static extern int dlclose(IntPtr handle);
    }
}
