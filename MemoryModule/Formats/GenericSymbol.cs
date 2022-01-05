using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats
{
    class GenericSymbol : ISymbol
    {
        public string Name { get; set; }

        public ulong Value { get; set; }

        public IntPtr Address { get; set; }
    }
}
