using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule.Windows
{
    [Flags]
    internal enum ImageSectionMemory : uint
    {
        Discardable = 0x02000000,
        NotCached = 0x04000000,
        NotPaged = 0x08000000,
        Shared = 0x10000000,
        Exectute = 0x20000000,
        Read = 0x40000000,
        Write = 0x80000000,
    }
}
