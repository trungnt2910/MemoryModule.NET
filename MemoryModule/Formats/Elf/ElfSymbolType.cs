using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    public enum ElfSymbolType : byte
    {
        None = 0,
        Object = 1,
        Function = 2,
        Section = 3,
        File = 4,
        Common = 5,
        TLS = 6,
        LoOS = 10,
        HiOS = 12,
        LoProc = 13,
        HiProc = 15,
    }
}
