using System;
using System.Collections.Generic;
using System.Text;

namespace GlibcInterop
{
    public unsafe class DtvSlotInfo
    {
        private DtvSlotInfoNative* _obj;
        
        public DtvSlotInfo(DtvSlotInfoNative* obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj must not be null");
            }
            _obj = obj;
        }

        public ulong Generation
        {
            get => (ulong)_obj->gen;
            set => _obj->gen = (UIntPtr)value;
        }

        public void* LinkMapPtr
        {
            get => _obj->map;
            set => _obj->map = value;
        }
    }
}
