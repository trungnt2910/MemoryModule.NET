using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats
{
    class GenericRebase : IRebase
    {
        public ulong AffectedAddress { get; set; }
        public ulong Addend { get; set; }
        public bool IgnoreExistingValue { get; set; }
    }
}
