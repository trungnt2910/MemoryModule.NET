using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats
{
    /// <summary>
    /// A generic class for formats that don't define its own binding structs.
    /// </summary>
    class GenericBind : IBind
    {
        public ulong AffectedAddress { get; set; }

        public string ModuleName { get; set; }

        public string SymbolName { get; set; }

        public ulong? SymbolIndex { get; set; }

        public ulong Addend { get; set; }
    }
}
