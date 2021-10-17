using MemoryModule.MacOS.Macho.Natives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    unsafe class MachoSectionArray : MachoManagedArray<MachoSectionNative, MachoSection>
    {
        public MachoSectionArray(byte* data, ulong offset, ulong count) : base(data, offset, count)
        {
        }

        protected override unsafe MachoSection Construct(void* ptr)
        {
            return new MachoSection(_memory, (ulong)((byte *)ptr - _memory));
        }
    }
}
