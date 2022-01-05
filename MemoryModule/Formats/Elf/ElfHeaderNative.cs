using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MemoryModule.Formats.Elf
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ElfHeaderNative
    {
        public fixed byte Magic[4];
        public ElfPointerClass Class;
        public ElfEndianess Endianess;
        public byte Version;
        public ElfAbi OsAbi;
        public byte AbiVersion;
        public fixed byte Padding[7];
        public ElfObjectType Type;
        public ElfArchitecture Machine;
        public int ElfVersion;
        public IntPtr Entry;
        public IntPtr ProgramHeaderOffset;
        public IntPtr SectionHeaderOffset;
        public int ArchitectureDependentFlags;
        public ushort ElfHeaderSize;
        public ushort ProgramHeaderSize;
        public ushort ProgramHeaderCount;
        public ushort SectionHeaderSize;
        public ushort SectionHeaderCount;
        public ushort SectionHeaderStringIndex;
    }
}
