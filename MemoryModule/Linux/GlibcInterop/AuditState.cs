using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AuditState
    {
        public UIntPtr cookie;
        public uint bindflags;
    }
}
