using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    public enum ElfPointerClass : byte
    {
        Pointer32 = 1,
        Pointer64 = 2
    }
}
