using System;

namespace MemoryModule.MacOS
{
    [Flags]
    public enum MmapProtectionFlags
    {
        Read = 0x1,
        Write = 0x2,
        Execute = 0x4,
        All = Read | Write | Execute,
        None = 0x0
    }
}