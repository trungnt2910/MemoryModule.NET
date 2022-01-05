using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats
{
    class GenericSection : ISection
    {
        public ulong MemoryOffset { get; set; }

        public ulong MemorySize { get; set; }

        public ulong FileOffset { get; set; }

        public ulong FileSize { get; set; }

        public MemoryProtection MemoryProtection { get; set; }

        public string Name { get; set; }

        public SectionType Type { get; set; }
    }
}
