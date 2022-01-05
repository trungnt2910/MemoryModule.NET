using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeSectionHeaderArray : MemoryValueObjectArray<PeSectionHeaderNative, PeSectionHeader>
    {
        public PeSectionHeaderArray(byte* memory, ulong offset, ulong size) : base(memory, offset, size)
        {
        }

        protected override unsafe PeSectionHeader Construct(byte* memory, ulong offset)
        {
            return new PeSectionHeader(memory, offset);
        }
    }
}
