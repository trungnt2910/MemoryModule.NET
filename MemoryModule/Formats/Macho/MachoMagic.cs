using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    enum MachoMagic : uint
    {
        Macho32Bit = 0xfeedface,
        Macho64Bit = 0xfeedfacf,
        MachoFat = 0xcafebabe,
        MachoFat64 = 0xcafebabf,
    }
}
