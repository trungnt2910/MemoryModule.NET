using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeSectionHeader : MemoryValueObject<PeSectionHeaderNative>, ISection
    {
        private string _name;
        private MemoryProtection _protection;
        private SectionType _type;

        public PeSectionHeader(byte* memory, ulong offset) : base(memory, offset)
        {
            if (_native->Characteristics.HasFlag(PeSectionFlags.CntCode))
            {
                _type = SectionType.Text;
            }

            _protection = 0;
            if (_native->Characteristics.HasFlag(PeSectionFlags.MemExecute))
            {
                _protection |= MemoryProtection.Execute;
            }
            if (_native->Characteristics.HasFlag(PeSectionFlags.MemWrite))
            {
                _protection |= MemoryProtection.Write;
            }
            if (_native->Characteristics.HasFlag(PeSectionFlags.MemRead))
            {
                _protection |= MemoryProtection.Read;
            }

            //An 8-byte, null-padded UTF-8 encoded string. If the string is exactly 8 characters long, there is no terminating null.
            //For longer names, this field contains a slash (/) that is followed by an ASCII representation of a decimal number that
            //is an offset into the string table.
            int zeroIndex;
            for (zeroIndex = 0; zeroIndex < 8; ++zeroIndex)
            {
                if (_native->Name[zeroIndex] == 0)
                {
                    break;
                }
            }
            _name = Marshal.PtrToStringAnsi((IntPtr)_native->Name, zeroIndex);
        }

        public ulong MemoryOffset => _native->VirtualAddress;

        public ulong MemorySize => _native->Misc.VirtualSize;

        public ulong FileOffset => _native->PointerToRawData;

        public ulong FileSize => _native->SizeOfRawData;

        public MemoryProtection MemoryProtection => _protection;

        public string Name => _name;

        public SectionType Type => _type; 
    }
}
