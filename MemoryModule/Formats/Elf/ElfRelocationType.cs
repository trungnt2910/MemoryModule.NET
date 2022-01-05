using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ElfRelocationType
    {
        private uint _value;

        public ElfRelocationType(ulong value)
        {
            _value = (uint)value;
        }

        public override string ToString()
        {
            return Environment.Is64BitProcess ?
                $"x86_64_{(x86_64)_value}" : $"x86_{(x86)_value}";
        }
        
        public bool IsNone()
        {
            return _value == None;
        }

        public bool IsRelative()
        {
            return _value == Relative;
        }

        public bool IsGlobDat()
        {
            return _value == GlobDat;
        }

        public bool IsJmpSlot()
        {
            return _value == JmpSlot;
        }

        public bool Is64()
        {
            if (!Environment.Is64BitProcess)
            {
                return false;
            }
            return _value == _64;
        }
        
        public bool Is16()
        {
            return _value == _16;
        }

        public bool IsModule()
        {
            return _value == Module;
        }

        public bool IsOffset()
        {
            return _value == Offset;
        }

        public static explicit operator ElfRelocationType(ulong value)
        {
            return new ElfRelocationType(value);
        }

        public static explicit operator uint(ElfRelocationType relocationType)
        {
            return relocationType._value;
        }

        public static explicit operator ulong(ElfRelocationType relocationType)
        {
            return relocationType._value;
        }

        public static readonly uint None =
            Environment.Is64BitProcess ? (uint)x86_64.None : (uint)x86.None;

        public static readonly uint Relative =
            Environment.Is64BitProcess ? (uint)x86_64.Relative : (uint)x86.Relative;

        public static readonly uint GlobDat =
            Environment.Is64BitProcess ? (uint)x86_64.GlobDat : (uint)x86.GlobDat;

        public static readonly uint JmpSlot =
            Environment.Is64BitProcess ? (uint)x86_64.JmpSlot : (uint)x86.JmpSlot;

        public static readonly uint _64 = (uint)x86_64._64;

        public static readonly uint _16 =
            Environment.Is64BitProcess ? (uint)x86_64._16 : (uint)x86._16;

        public static readonly uint Module =
            Environment.Is64BitProcess ? (uint)x86_64.DTPMOD64 : (uint)x86.DTPMOD32;

        public static readonly uint Offset =
            Environment.Is64BitProcess ? (uint)x86_64.DTPOFF64 : (uint)x86.DTPOFF32;

        private enum x86 : uint
        {
            // x86
            None = 0, //None
            _32 = 1, //word32 S + A
            PC32 = 2, //word32 S + A - P
            GOT32 = 3, //word32 G + A
            PLT32 = 4, //word32 L + A - P
            Copy = 5, //None
            GlobDat = 6, //S
            JmpSlot = 7, //word32 S
            Relative = 8, //B + A
            GOTOff = 9, //S + A - GOT
            GOTPC = 10, // GOT + A - P
            _32PLT = 11, //L + A
            _16 = 20, //word16 S + A
            PC16 = 21, // word16 S + A - P
            _8 = 22, //word8 S + A
            PC8 = 23, //word8 S + A - P
            DTPMOD32 = 35,
            DTPOFF32 = 36,
            Size32 = 38, //word32 Z + A
        }

        private enum x86_64 : uint
        {
            //64
            None = 0, //None
            _64 = 1, //word64 S + A
            PC32 = 2, //word32 S + A - P
            GOT32 = 3, //word32 G + A
            PLT32 = 4, //word32 L + A - P
            Copy = 5, //None
            GlobDat = 6, //word64 S
            JmpSlot = 7, //word64 S
            Relative = 8, //word64 B + A
            GOTPCREL = 9, //word32 G + GOT + A - P
            _32 = 10, // word32 S + A
            _32S = 11, // word32 S + A
            _16 = 12, // word16 S + A
            PC16 = 13, // word16 S + A - P
            _8 = 14, // word8 S + A
            PC8 = 15, // word8 S + A - P
            DTPMOD64 = 16, // word64 
            DTPOFF64 = 17, // word64
            PC64 = 24, // word64 S + A - P
            GOTOff64 = 25, //word64 S + A - GOT
            GOTPC32 = 26, //word32 GOT + A + P
            Size32 = 32, // word32 Z + A
            Size64 = 33, // word64 Z + A
        }
    }
}
