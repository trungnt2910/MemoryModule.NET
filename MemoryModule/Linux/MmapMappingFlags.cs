using System;

namespace MemoryModule.Linux
{
    [Flags]
    internal enum MmapMappingFlags : int
    {
        Private = 0x02,
        Fixed = 0x10,
        Anonymous = 0x20,
    }
}