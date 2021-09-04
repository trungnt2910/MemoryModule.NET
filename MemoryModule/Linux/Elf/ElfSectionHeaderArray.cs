using MemoryModule.Linux.Elf;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MemoryModule.Linux.Elf
{
    internal unsafe class ElfSectionHeaderArray : IReadOnlyList<ElfSectionHeader>
    {
        private byte* memory;
        private ElfSectionHeaderNative* first;
        private int count;
        private byte* nameData;

        public ElfSectionHeaderArray(byte* data, ulong offset, int count, int nameIndex = -1)
        {
            memory = data;
            first = (ElfSectionHeaderNative*)(data + offset);
            this.count = count;
            if (nameIndex != 0)
            {
                nameData = new ElfSectionHeader(memory, first + nameIndex).GetData();
            }
        }

        public ElfSectionHeader this[int index]
        {
            get
            {
                if (index >= count)
                {
                    throw new IndexOutOfRangeException($"Index {index} out of range {count}");
                }

                return new ElfSectionHeader(memory, first + index, nameData + first[index].Name);
            }
        }

        public int Count => count;

        public IEnumerator<ElfSectionHeader> GetEnumerator()
        {
            for (int i = 0; i < count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < count; ++i)
            {
                yield return this[i];
            }
        }
    }

}