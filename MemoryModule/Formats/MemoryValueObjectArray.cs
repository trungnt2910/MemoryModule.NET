using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats
{
    /// <summary>
    /// A contiguous array of unmanaged objects, wrapped by a <see cref="MemoryValueObject{T}"/>
    /// </summary>
    /// <typeparam name="T">The native type.</typeparam>
    /// <typeparam name="TManaged">The corresponding managed type</typeparam>
    unsafe abstract class MemoryValueObjectArray<T, TManaged> : MemoryObject, IReadOnlyList<TManaged> 
        where T : unmanaged
        where TManaged : MemoryValueObject<T>
    {
        protected ulong _count;
        protected TManaged[] _arr;

        /// <summary>
        /// Creates an array, with a base of <paramref name="memory"/>, starting at <paramref name="offset"/>, and can hold
        /// <paramref name="size"/> native elements.
        /// </summary>
        /// <param name="memory">The base memory</param>
        /// <param name="offset">The offset</param>
        /// <param name="size">The size, in ELEMENTS, not bytes.</param>
        protected MemoryValueObjectArray(byte* memory, ulong offset, ulong size) : base(memory, offset)
        {
            _count = size;
            _arr = (TManaged[])Array.CreateInstance(typeof(TManaged), (long)_count);
        }

        protected abstract TManaged Construct(byte* memory, ulong offset);

        public TManaged this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                return this[(ulong)index];
            }
        }

        public TManaged this[ulong index]
        {
            get
            {
                // This checked bounds for us.
                if (_arr[index] == null)
                {
                    _arr[index] = Construct(_memory, (ulong)(_data - _memory) + (ulong)sizeof(T) * index);
                }
                return _arr[index];
            }
        }

        public int Count => ((IReadOnlyCollection<TManaged>)_arr).Count;

        public IEnumerator<TManaged> GetEnumerator()
        {
            // Elements might be null on first enumeration.
            for (ulong i = 0; i < _count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _arr.GetEnumerator();
        }
    }

    unsafe abstract class MemoryValueObjectArray<T> : MemoryValueObjectArray<T, MemoryValueObject<T>>
        where T: unmanaged
    {
        protected MemoryValueObjectArray(byte* memory, ulong offset, ulong size) : base(memory, offset, size)
        {
        }
    }

}
