using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct PeHintNameTableEntryNative
    {
        public ushort Hint;
        // It's not 1, it's actually a variable-length, null-terminated char array.
        public fixed byte Name[1];
    }
}
