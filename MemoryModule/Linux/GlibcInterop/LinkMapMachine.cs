using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct link_map_machine
    {
        UIntPtr plt;
        UIntPtr gotplt;
        void* tlsdesc_table;
    }
}
