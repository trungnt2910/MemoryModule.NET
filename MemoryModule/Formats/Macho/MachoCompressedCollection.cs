using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    /// <summary>
    /// A collection of a data type that are compressed in Macho images.
    /// </summary>
    /// <typeparam name="T">The represented data type</typeparam>
    unsafe abstract class MachoCompressedCollection<T> : IReadOnlyCollection<T>
    {
        private readonly List<T> _binds;

        protected byte* _memory;
        protected byte* _data;
        protected ulong _size;

        public MachoCompressedCollection(byte* memory, ulong offset, ulong size)
        {
            _memory = memory;
            _data = _memory + offset;
            _size = size;
            _binds = Decompress();
        }

        public int Count => ((IReadOnlyCollection<T>)_binds).Count;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_binds).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_binds).GetEnumerator();
        }

        protected abstract List<T> Decompress();

        protected const byte Low7Bits = (1 << 7) - 1;

        protected static ulong ReadUleb128(ref byte* ptr)
        {
            ulong result = 0;
            int shift = 0;
            while (true)
            {
                var current = *ptr;
                result |= ((ulong)(current & Low7Bits)) << shift;
                if (current < Low7Bits)
                    break;
                shift += 7;
                ++ptr;
            }
            ++ptr;
            return result;
        }

        protected static long ReadSleb128(ref byte* ptr)
        {
            ulong result = 0;
            int shift = 0;
            while (true)
            {
                var current = *ptr;
                result |= ((ulong)(current & Low7Bits)) << shift;
                if (current < Low7Bits)
                {
                    if (((current >> 6) & 1) == 1)
                    {
                        shift += 7;
                        if (shift < 64)
                        {
                            result |= ~0ul << shift;
                        }
                    }
                    break;
                }
                shift += 7;
                ++ptr;
            }
            ++ptr;
            return unchecked((long)result);
        }

        //protected static string ReadAscii(ref byte* ptr)
        //{
        //    var sb = new StringBuilder();
        //    while (*ptr != 0)
        //    {
        //        sb.Append((char)*ptr);
        //        ++ptr;
        //    }
        //    ++ptr;
        //    return sb.ToString();
        //}

        protected static string ReadUtf8(ref byte* ptr)
        {
            byte* startPtr = ptr;
            while (*ptr != 0)
            {
                ++ptr;
            }
            ++ptr;
            return new string((sbyte*)startPtr, 0, (int)(ptr - startPtr) - 1, Encoding.UTF8);
        }
    }
}
