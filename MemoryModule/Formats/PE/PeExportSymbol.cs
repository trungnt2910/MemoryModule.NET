using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    class PeExportSymbol : ISymbol
    {
        // Name.
        public string Name { get; set; }

        // Will contain the ordinal.
        public ulong Value { get; set; }

        // This is the address.
        public IntPtr Address { get; set; }
    }
}
