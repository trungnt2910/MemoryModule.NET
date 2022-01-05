using System;
using System.Runtime.InteropServices;

namespace MemoryModule.Formats.Elf
{
    public unsafe class ElfStringTable
    {
        private byte* _memory;
        private byte* _start;

        public ElfStringTable(byte* memory, ulong offset)
        {
            _memory = memory;
            _start = _memory + offset;
        }

        public string GetString(ulong offset)
        {
            return Marshal.PtrToStringAnsi((IntPtr)(_start + offset));
        }
    }
}