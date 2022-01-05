using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Tls
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr TlsHookFunctionDelegate(IntPtr input);
}
