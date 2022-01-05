using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ElfDynamicSectionItemNative
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct DummyUnion
        {
            [FieldOffset(0)]
            public UIntPtr Value;
            [FieldOffset(0)]
            public UIntPtr Ptr;
        }

        public UIntPtr Tag;
        public DummyUnion ValueOrPtr;
    }
}
