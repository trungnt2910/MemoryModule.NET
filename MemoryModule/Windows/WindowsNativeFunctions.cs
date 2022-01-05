using MemoryModule.Abstractions;
using System;
using System.Runtime.InteropServices;

namespace MemoryModule.Windows
{
    class WindowsNativeFunctions : NativeFunctions
    {
        public override bool FreeLibrary(IntPtr handle)
        {
            return FreeLibrary_(handle);
        }

        public override IntPtr GetSymbolFromLibrary(IntPtr handle, string name)
        {
            return GetProcAddress(handle, name);
        }

        public override IntPtr GetSymbolFromLibrary(IntPtr handle, IntPtr nameValue)
        {
            return GetProcAddress(handle, nameValue);
        }

        public override IntPtr LoadLibrary(string name)
        {
            return LoadLibraryA(name);
        }

        public override IntPtr VirtualAllocate(IntPtr hint, ulong size, MemoryProtection protection)
        {
            var flag = MemoryProtectionToNativePageProtection(protection);

            var result = VirtualAlloc(hint, (UIntPtr)size, MemoryAllocation.Commit | MemoryAllocation.Reserve, flag);

            // The memory at hint is already allocated.
            if (result == IntPtr.Zero)
            {
                result = VirtualAlloc(IntPtr.Zero, (UIntPtr)size, MemoryAllocation.Commit | MemoryAllocation.Reserve, flag);
            }

            return result;
        }

        public override bool VirtualFree(IntPtr addr, ulong size)
        {
            return VirtualFree(addr, UIntPtr.Zero, MemoryAllocation.Decommit | MemoryAllocation.Release);
        }

        public override bool VirtualProtect(IntPtr addr, ulong size, MemoryProtection protection)
        {
            var flag = MemoryProtectionToNativePageProtection(protection);
            unsafe
            {
                return VirtualProtect(addr, (UIntPtr)size, flag, (IntPtr)(&flag)) != 0;
            }
        }

        private static PageProtection MemoryProtectionToNativePageProtection(MemoryProtection protection)
        {
            return ProtectionFlags
                [protection.HasFlag(MemoryProtection.Execute) == false ? 0 : 1]
                [protection.HasFlag(MemoryProtection.Read) == false ? 0 : 1]
                [protection.HasFlag(MemoryProtection.Write) == false ? 0 : 1];
        }

        // Protection flags for memory pages (Executable, Readable, Writeable)
        private static readonly PageProtection[][][] ProtectionFlags = new PageProtection[][][]
        {
            new PageProtection[][]
            {
                // not executable
                new [] {PageProtection.NoAccess, PageProtection.ReadWrite},
                new [] {PageProtection.ReadOnly, PageProtection.ReadWrite},
            },
            new PageProtection[][]
            {
                // executable
                new [] { PageProtection.Execute, PageProtection.ExecuteReadWrite},
                new [] { PageProtection.ExecuteRead, PageProtection.ExecuteReadWrite},
            },
        };

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, MemoryAllocation flAllocationType, PageProtection flProtect);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern bool VirtualFree(IntPtr lpAddress, UIntPtr dwSize, MemoryAllocation dwFreeType);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, PageProtection flNewProtect, IntPtr lpflOldProtect);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        // The library name seems to always be ANSI, so we must use LoadLibraryA in this case.
        private static extern IntPtr LoadLibraryA([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, IntPtr lpProcName);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, EntryPoint = "FreeLibrary")]
        private static extern bool FreeLibrary_(IntPtr hLibModule);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void GetNativeSystemInfo(IntPtr lpSystemInfo);
    }
}
