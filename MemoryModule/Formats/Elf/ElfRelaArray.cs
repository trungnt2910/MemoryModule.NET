using MemoryModule.Formats.Elf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    internal unsafe class ElfRelaArray : ElfManagedArray<ElfRelaNative, ElfRela>
    {
        public ElfRelaArray(byte* data, ulong offset, ulong count) : base(data, offset, count)
        {
        }

        protected override unsafe ElfRela Construct(void* ptr)
        {
            return new ElfRela(_memory, (ElfRelaNative*)ptr);
        }
    }
}
