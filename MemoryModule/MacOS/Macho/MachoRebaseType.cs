using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    enum MachoRebaseType
    {
        Pointer = 1,
        TextAbsolute32 = 2,
        TextPcrel32 = 3,
    }
}
