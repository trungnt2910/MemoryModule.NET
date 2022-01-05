using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats
{
    /// <summary>
    /// An object that's represented by an offset to a base address.
    /// </summary>
    unsafe abstract class MemoryObject
    {
        protected byte* _memory;
        protected byte* _data;

        public MemoryObject(byte* memory, ulong offset)
        {
            _memory = memory;
            _data = _memory + offset;
        }

        /// <summary>
        /// Gets an address <paramref name="offset"/> bytes from the base of this object.
        /// </summary>
        /// <param name="offset">The desired offset</param>
        /// <returns>The address</returns>
        /// <remarks>This function is useful for purposes such as resolving string tables in some executable formats.</remarks>
        public byte* GetAddress(ulong offset = 0)
        {
            return _memory + offset;
        }
    }
}
