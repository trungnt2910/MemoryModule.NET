using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule
{
    [Flags]
    enum Memory : uint
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
    }
}
