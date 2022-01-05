using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ElfProgramHeaderNative
    {
        // As the different between 8 bytes and 
        // 4 bytes has been addressed using UIntPtr's,
        // the program's header should be correct, even on 32-bit mode.

        // The alignment is retarded.
        // For the 32-bit data layout, we must stil use
        // uint's, as the 64-bit's alignment might make 
        // pointers grow too big.
        // For the 64 bit, we MUST use UIntPtr to make 
        // sure they shrink when running on x86.
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DataLayout32
        {
            public ElfProgramHeaderType Type;
            public uint Offset;
            public uint VirtualAddress;
            public uint PhysicalAddress;
            public uint FileSize;
            public uint MemorySize;
            public uint Flags;
            public uint Align;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DataLayout64
        {
            public ElfProgramHeaderType Type;
            public uint Flags;
            public UIntPtr Offset;
            public UIntPtr VirtualAddress;
            public UIntPtr PhysicalAddress;
            public UIntPtr FileSize;
            public UIntPtr MemorySize;
            public UIntPtr Align;
        }

        [FieldOffset(0)]
        public DataLayout32 Data32;
        
        [FieldOffset(0)]
        public DataLayout64 Data64;
    }
}
