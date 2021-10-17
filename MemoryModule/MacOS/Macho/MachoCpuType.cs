using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    enum MachoCpuType : uint
    {
        ABI64 = 0x01000000,
        ABI32 = 0x02000000,
        Any = unchecked((uint)-1),
        MC680X0 = 0x06,
        I386 = 0x07,
        X86_64 = I386 | ABI64,
        ARM = 0x0c,
        MC88000 = 0xd,
        ARM64 = ARM | ABI64,
        ARM64_32 = ARM | ABI32,
        PowerPC = 0x12,
        PowerPC64 = PowerPC | ABI64
    }
}
