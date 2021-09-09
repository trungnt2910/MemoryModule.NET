using MemoryModule.Linux.Elf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Linux.Elf
{
    internal unsafe class ElfDynamicSectionArray : ElfManagedArray<ElfDynamicSectionItemNative, ElfDynamicSectionItem>
    {
        // A null ElfDynamicSectionItem marks the end of the array.
        public ElfDynamicSectionArray(byte* data, ulong offset) : base(data, offset, 0)
        {
            ulong _count = 0;
            while (((ElfDynamicSectionItemNative *)_first)[_count].Tag != (UIntPtr)ElfDynamicSectionItemType.Null)
            {
                ++_count;
            }

            // Allows accessing the last element.
            ++_count;

            Count = _count;
        }

        protected override ElfDynamicSectionItem Construct(void* ptr)
        {
            return new ElfDynamicSectionItem(_memory, (ElfDynamicSectionItemNative*)ptr);
        }
    }
}
