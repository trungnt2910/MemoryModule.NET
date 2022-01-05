using MemoryModule.Formats.Elf;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MemoryModule.Formats.Elf
{
    unsafe class ElfProgramHeaderArray : IReadOnlyList<ElfProgramHeader>
    {
        private byte* _memory;
        private ElfProgramHeaderNative* _first;
        private int _count;

        public ElfProgramHeaderArray(byte* data, ulong offset, int count)
        {
            _memory = data;
            _first = (ElfProgramHeaderNative*)(data + offset);
            _count = count;
        }

        public ElfProgramHeader this[int index]
        {
            get
            {
                if (index >= _count)
                {
                    throw new IndexOutOfRangeException($"Index {index} out of range {_count}");
                }

                return new ElfProgramHeader(_memory, _first + index);
            }
        }

        public int Count => _count;

        public IEnumerator<ElfProgramHeader> GetEnumerator()
        {
            for (int i = 0; i < _count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _count; ++i)
            {
                yield return this[i];
            }
        }
    }
}