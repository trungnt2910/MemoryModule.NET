using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeHintNameTableEntry : MemoryValueObject<PeHintNameTableEntryNative>
    {
        public PeHintNameTableEntry(byte* memory, ulong offset) : base(memory, offset)
        {
            Name = Marshal.PtrToStringAnsi((IntPtr)_native->Name);
        }

        public string Name { get; private set; }
    }
}
