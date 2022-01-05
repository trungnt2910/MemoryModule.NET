using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ElfSectionHeaderNative
    {
        /// <summary>
        /// An offset to a string in the .shstrtab section that represents the name of this section.
        /// </summary>
        public uint Name;
        public ElfSectionHeaderType Type;
        public UIntPtr Flags;
        public UIntPtr VirtualAddress;
        public UIntPtr Offset;
        public UIntPtr Size;
        public uint Link;
        public uint Info;
        public UIntPtr Alignment;
        public UIntPtr EntrySize;
    }
}
