using System;

namespace MemoryModule.Abstractions
{
    /// <summary>
    /// Platform independent memory protection flags.
    /// This should be mapped to mprotect or VirtualProtect flags by implementations.
    /// </summary>
    [Flags]
    public enum MemoryProtection
    {
        Read = 0x1,
        Write = 0x2,
        Execute = 0x4
    }
}