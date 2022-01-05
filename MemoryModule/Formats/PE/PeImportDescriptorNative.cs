using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    [StructLayout(LayoutKind.Explicit)]
    struct PeImportDescriptorNative
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
}
