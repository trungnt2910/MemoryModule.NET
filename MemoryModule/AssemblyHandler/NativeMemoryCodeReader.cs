using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.AssemblyHandler
{
    class NativeMemoryCodeReader : CodeReader
    {
        private IntPtr _mem;
        private ulong count;

        public NativeMemoryCodeReader(IntPtr position, ulong size)
        {
            _mem = position;
            count = size;
        }

        public override int ReadByte()
        {
            if (count == 0)
            {
                return -1;
            }

            var b = Marshal.ReadByte(_mem);
            _mem += 1;
            --count;

            return b;
        }
    }
}
