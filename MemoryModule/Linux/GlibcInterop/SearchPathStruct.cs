using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct r_search_path_struct
    {
        public void** dirs; //r_search_path_elem*
        public int malloced;
    }
}
