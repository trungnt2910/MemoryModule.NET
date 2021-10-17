using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    enum MachoBindType : byte
    {
        Pointer = 1,
        TextAbsolute32 = 2,
        TextPcrel32 = 3,
    }
}
