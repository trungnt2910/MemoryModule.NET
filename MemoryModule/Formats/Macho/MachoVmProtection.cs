using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    [Flags]
    enum MachoVmProtection : uint
    {
        Read = 0x1,
        Write = 0x2,
        Execute = 0x4,
    }
}
