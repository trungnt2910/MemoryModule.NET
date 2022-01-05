using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeDataDirectoryArray : MemoryValueObjectArray<PeDataDirectoryNative, PeDataDirectory>
    {
        public PeDataDirectoryArray(byte* memory, ulong offset, ulong size) : base(memory, offset, size)
        {
        }

        public PeDataDirectory this[PeDirectoryEntryType type] => this[(ulong)type];

        protected override unsafe PeDataDirectory Construct(byte* memory, ulong offset)
        {
            return new PeDataDirectory(memory, offset);
        }
    }
}
