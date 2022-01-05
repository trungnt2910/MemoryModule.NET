using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    enum MachoFileType : uint
    {
        Object = 0x1,
        Execute = 0x2,
        Fvmlib = 0x3,
        Core = 0x4,
        Preload = 0x5,
        Dylib = 0x6,
        Dylinker = 0x7,
        Bundle = 0x8,
        DylibStub = 0x9,
        Dsym = 0xa,
        KextBundle = 0xb,
    }
}
