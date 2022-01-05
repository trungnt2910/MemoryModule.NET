using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats
{
    /// <summary>
    /// A MemoryObject that wraps around an umanaged struct <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The corresponding native object type.</typeparam>
    unsafe class MemoryValueObject<T> : MemoryObject where T: unmanaged
    {
        protected T* _native => (T*)_data;

        public MemoryValueObject(byte* memory, ulong offset) : base(memory, offset)
        {
        }
    }
}
