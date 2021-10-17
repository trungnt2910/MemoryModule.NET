using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MemoryModule.MacOS.Macho
{
    internal abstract unsafe class MachoManagedArray<TNative, TManaged> : IEnumerable, IEnumerable<TManaged>
         where TNative : struct
         where TManaged : class
    {
        private static readonly ulong _nativeSize = (ulong)Marshal.SizeOf<TNative>();

        private ulong _count;
        protected byte* _memory;
        protected byte* _first;

        protected TManaged[] _managed;

        public MachoManagedArray(byte* data, ulong offset, ulong count)
        {
            _memory = data;
            _first = data + offset;
            _count = count;
            _managed = new TManaged[count];
        }

        public TManaged this[ulong index]
        {
            get
            {
                if (index > _count)
                {
                    throw new IndexOutOfRangeException($"{index} is greater than array range {_count}");
                }

                return _managed[index] = _managed[index] ?? Construct(_first + index * _nativeSize);
            }
        }

        public ulong Count
        {
            get => _count;
            protected set
            {
                _count = value;
                _managed = new TManaged[_count];
            }
        }

        public IEnumerator<TManaged> GetEnumerator()
        {
            for (ulong i = 0; i < _count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (ulong i = 0; i < _count; ++i)
            {
                yield return this[i];
            }
        }

        protected abstract TManaged Construct(void* ptr);
    }
}
