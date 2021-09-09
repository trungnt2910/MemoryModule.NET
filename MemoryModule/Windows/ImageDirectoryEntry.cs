using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Windows
{
    internal enum ImageDirectoryEntry
    {
        Import = 1,
        BaseReloc = 5,
        Tls = 9,
        Export = 0,
    }
}
