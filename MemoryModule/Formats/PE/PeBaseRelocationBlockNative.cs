using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    [StructLayout(LayoutKind.Sequential)]
    struct PeBaseRelocationBlockNative
    {
        public uint VirtualAddress;
        public uint SizeOfBlock;
    }
}
