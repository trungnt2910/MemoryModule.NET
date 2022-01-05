using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    [Flags]
    public enum ElfProgramHeaderLoadFlags
    {
        Execute = 0x1,
        Write = 0x2,
        Read = 0x4,
    }
}
