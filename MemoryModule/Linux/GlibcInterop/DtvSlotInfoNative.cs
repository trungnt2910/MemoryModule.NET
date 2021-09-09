using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GlibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DtvSlotInfoNative
    {
        public UIntPtr gen;
        public void* map;
    }
}
