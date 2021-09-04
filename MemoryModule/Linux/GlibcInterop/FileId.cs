using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct r_file_id
    {
        ulong dev; //dev_t
        ulong ino; //ino64_t
    }
}
