using System;
using System.Collections.Generic;
using System.Text;

namespace GlibcInterop
{
    public unsafe class DtvSlotInfoArray : ManagedArray<DtvSlotInfoNative, DtvSlotInfo>
    {
        public DtvSlotInfoArray(byte* data, ulong count) : base(data, count)
        {

        }

        public DtvSlotInfoArray(IntPtr managedPtr) :
            // The array starts after the struct.
            this((byte*)managedPtr + sizeof(DtvSlotInfoListNative), (ulong)((DtvSlotInfoListNative*)managedPtr)->len)
        {

        }

        protected override unsafe DtvSlotInfo Construct(void* ptr)
        {
            return new DtvSlotInfo((DtvSlotInfoNative *)ptr);
        }
    }
}
