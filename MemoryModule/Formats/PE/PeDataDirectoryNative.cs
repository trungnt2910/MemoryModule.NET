using System.Runtime.InteropServices;

namespace MemoryModule.Formats.PE
{
    [StructLayout(LayoutKind.Sequential)]
    struct PeDataDirectoryNative
    {
        public uint VirtualAddress;
        public uint Size;
    }
}