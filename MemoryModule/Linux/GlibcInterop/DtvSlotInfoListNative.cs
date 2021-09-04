using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DtvSlotInfoListNative
    {
        public UIntPtr len;
        public DtvSlotInfoListNative* next;
        // That is not a pointer! It's a fucking variable-lengthed array.
        // Really, I don't know why it's allowed. That's too evil!
        //public DtvSlotInfoNative* slotinfo;
    }
}
