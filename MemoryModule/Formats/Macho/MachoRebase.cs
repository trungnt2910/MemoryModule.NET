using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    class MachoRebase
    {
        //seg-index, seg-offset, type
        public ulong SegmentIndex { get; internal set; }
        public ulong SegmentOffset { get; internal set; }
        public MachoRebaseType Type { get; internal set; }

        public object Clone()
        {
            return new MachoRebase()
            {
                SegmentIndex = SegmentIndex,
                SegmentOffset = SegmentOffset,
                Type = Type,
            };
        }

        public override string ToString()
        {
            return
$@"Segment Index:    {SegmentIndex},
Segment Offset:   0x{SegmentOffset:x},
Type:             {Type}";
        }
    }
}
