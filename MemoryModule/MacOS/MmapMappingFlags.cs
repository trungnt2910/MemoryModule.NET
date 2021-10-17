using System;

namespace MemoryModule.MacOS
{
    [Flags]
    internal enum MmapMappingFlags : int
    {
        Private = 0x02,
        Fixed = 0x10,
        // Different from Linux, which is 0x20
        Anonymous = 0x1000,
    }
}