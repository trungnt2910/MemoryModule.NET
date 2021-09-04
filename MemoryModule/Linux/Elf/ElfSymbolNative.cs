using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Linux.Elf
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ElfSymbolNative
    {
        public struct DataLayout64
        {
            public uint Name;
            public byte Info;
            public byte Other;
            public ushort SectionHeaderTableIndex;
            public UIntPtr Value;
            public UIntPtr Size;
        }

        public struct DataLayout32
        {
            public uint Name;
            public uint Value;
            public uint Size;
            public byte Info;
            public byte Other;
            public ushort SectionHeaderTableIndex;
        }

        [FieldOffset(0)]
        public DataLayout32 Data32;

        [FieldOffset(0)]
        public DataLayout64 Data64;
    }
}
