using System;

namespace MemoryModule.MacOS.Macho
{
    class MachoBind : ICloneable
    {
        public ulong SegmentIndex { get; internal set; }
        public ulong SegmentOffset { get; internal set; }
        public MachoBindType Type { get; internal set; }
        public ulong LibraryOrdinal { get; internal set; }
        public string Name { get; internal set; }
        public ulong Addend { get; internal set; }

        public object Clone()
        {
            return new MachoBind()
            {
                SegmentIndex = SegmentIndex,
                SegmentOffset = SegmentOffset,
                Type = Type,
                LibraryOrdinal = LibraryOrdinal,
                Name = Name,
                Addend = Addend
            };
        }

        public override string ToString()
        {
            return
$@"Segment Index:    {SegmentIndex},
Segment Offset:   0x{SegmentOffset:x},
Type:             {Type},
Library Ordinal:  {LibraryOrdinal},
Name:             {Name},
Addend:           0x{Addend:x}";
        }
    }
}