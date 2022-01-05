using MemoryModule.Formats.Elf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    internal unsafe class ElfSymbolArray : ElfManagedArray<ElfSymbolNative, ElfSymbol>
    {
        public ElfSymbolArray(byte* data, ulong offset, ulong count) : base(data, offset, count)
        {
        }

        protected override unsafe ElfSymbol Construct(void* ptr)
        {
            return new ElfSymbol(_memory, (ElfSymbolNative*)ptr);
        }
    }
}
