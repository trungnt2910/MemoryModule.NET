using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Tls
{
    class TlsGlobalDescriptor
    {
        public ulong Generation;
        public ulong InitSize;
        public ulong Size;
        public IntPtr Address;
    }
}
