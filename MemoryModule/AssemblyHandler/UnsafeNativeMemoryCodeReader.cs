using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.AssemblyHandler
{
    unsafe class UnsafeNativeMemoryCodeReader : CodeReader
    {
        private byte* _begin;
        private readonly byte* _end;

        public UnsafeNativeMemoryCodeReader(byte* position, ulong size)
        {
            _begin = position;
            _end = _begin + size;
        }

        public override int ReadByte()
        {
            if (_begin == _end)
            {
                return -1;
            }

            var temp = *_begin;
            ++_begin;

            return temp;
        }
    }
}
