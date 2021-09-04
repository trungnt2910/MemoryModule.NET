using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct r_debug
    {
        int r_version;
        void* r_map; //link_map_public

        UIntPtr r_brk;
        int r_state;

        UIntPtr r_ldbase;
    };
}
