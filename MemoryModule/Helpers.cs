using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MemoryModule
{
    internal static class Helpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MmapProtectionFlags WindowsToUnixProtection(PageProtection flags)
        {
            switch (flags)
            {
                case PageProtection.NoAccess:
                    return MmapProtectionFlags.None;
                case PageProtection.Execute:
                    return MmapProtectionFlags.Execute;
                case PageProtection.ReadOnly:
                    return MmapProtectionFlags.Read;
                case PageProtection.WriteCopy:
                    return MmapProtectionFlags.Write;
                case PageProtection.ReadWrite:
                    return MmapProtectionFlags.Read | MmapProtectionFlags.Write;
                case PageProtection.ExecuteRead:
                    return MmapProtectionFlags.Execute | MmapProtectionFlags.Read;
                case PageProtection.ExecuteWriteCopy:
                    return MmapProtectionFlags.Execute | MmapProtectionFlags.Write;
                case PageProtection.ExecuteReadWrite:
                    return MmapProtectionFlags.All;
                default:
                    return MmapProtectionFlags.None;
            }
        }
    }
}
