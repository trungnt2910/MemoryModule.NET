using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct PeSectionHeaderNative
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct MiscUnion
        {
            [FieldOffset(0)]
            public uint PhysicalAddress;
            [FieldOffset(0)]
            public uint VirtualSize;
        }

        public fixed byte Name[8];
        public MiscUnion Misc;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public PeSectionFlags Characteristics;
    }
}
