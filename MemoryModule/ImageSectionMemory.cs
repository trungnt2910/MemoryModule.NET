using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule
{
    [Flags]
    enum ImageSectionMemory : uint
    {
//#define IMAGE_SCN_MEM_DISCARDABLE 0x02000000
        Discardable = 0x02000000,
        //#define IMAGE_SCN_MEM_NOT_CACHED 0x04000000
        NotCached = 0x04000000,
        //#define IMAGE_SCN_MEM_NOT_PAGED 0x08000000
        NotPaged = 0x08000000,
        //#define IMAGE_SCN_MEM_SHARED 0x10000000
        Shared = 0x10000000,
        //#define IMAGE_SCN_MEM_EXECUTE 0x20000000
        Exectute = 0x20000000,
        //#define IMAGE_SCN_MEM_READ 0x40000000
        Read = 0x40000000,
        //#define IMAGE_SCN_MEM_WRITE 0x80000000
        Write = 0x80000000,
    }
}
