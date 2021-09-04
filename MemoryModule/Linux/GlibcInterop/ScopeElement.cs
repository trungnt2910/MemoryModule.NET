using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ScopeElement
    {
        public LinkMap* r_list;
        public uint r_nlist;
    }
}
