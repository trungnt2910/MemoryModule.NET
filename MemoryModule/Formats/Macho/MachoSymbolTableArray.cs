using MemoryModule.Formats.Macho.Natives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    unsafe class MachoSymbolTableArray : MachoManagedArray<MachoSymbolTableEntryNative, MachoSymbolTableEntry>
    {
        public MachoSymbolTableArray(byte* data, ulong offset, ulong count) : base(data, offset, count)
        {
        }

        protected override unsafe MachoSymbolTableEntry Construct(void* ptr)
        {
            return new MachoSymbolTableEntry(_memory, (ulong)((byte*)ptr - _memory));
        }
    }
}
