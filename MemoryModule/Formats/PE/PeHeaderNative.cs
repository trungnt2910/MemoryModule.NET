using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct PeHeaderNative
    {
        public uint Signature;
        public PeCoffHeaderNative FileHeader;
        public PeOptionalHeaderNative OptionalHeader;
    }
}
