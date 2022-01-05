using MemoryModule.Formats.Macho.Natives;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    unsafe class MachoSection
    {
        const uint SectionTypeMask = 0x000000ff; /* 256 section types */

        private static readonly ulong _sectnameOffset = (ulong)Marshal.OffsetOf<MachoSectionNative>(nameof(MachoSectionNative.sectname));
        private static readonly ulong _segnameOffset = (ulong)Marshal.OffsetOf<MachoSectionNative>(nameof(MachoSectionNative.segname));

        private byte* _memory;
        private byte* _data;
        private string _sectname;
        private string _segname;

        private MachoSectionNative* Section => (MachoSectionNative*)_data;

        public MachoSection(byte* memory, ulong offset)
        {
            _memory = memory;
            _data = _memory + offset;

            _sectname = Marshal.PtrToStringAnsi((IntPtr)(_data + _sectnameOffset));
            _segname = Marshal.PtrToStringAnsi((IntPtr)(_data + _segnameOffset));
        }

        public string SectionName => _sectname;
        public string SegmentName => _segname;
        public ulong MemoryAddress => (ulong)Section->addr;
        public ulong MemorySize => (ulong)Section->size;
        public uint FileOffset => Section->offset;
        public uint Alignment => Section->align;
        public uint RelocOffset => Section->reloff;
        public uint RelocCount => Section->nreloc;
        public MachoSectionType Type => (MachoSectionType)(Section->flags & SectionTypeMask);

        public override string ToString()
        {
            return
$@"Section Name:   {SectionName},
Segment Name:   {SegmentName},
Type:           {Type},
Memory Address: 0x{(ulong)MemoryAddress:x},
Memory Size:    0x{MemorySize:x},
File Offset:    0x{FileOffset:x},
Alignment:      0x{Alignment:x},
Reloc Offset:   0x{RelocOffset},
Reloc Count:    {RelocCount}";
        }
    }
}
