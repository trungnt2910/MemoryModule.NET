using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    [Flags]
    enum MachoExportSymbolFlags : uint
    {
        KindMask = 0x03,
        KindRegular = 0x00,
        KindThreadLocal = 0x01,
        WeakDefinition = 0x04,
        Reexport = 0x08,
        StubAndResolver = 0x10,
    }
}
