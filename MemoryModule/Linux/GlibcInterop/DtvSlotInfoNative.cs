using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DtvSlotInfoNative
    {
        public UIntPtr gen;
        public LinkMap* map;
    }
}
