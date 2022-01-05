using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeDataDirectory : MemoryValueObject<PeDataDirectoryNative>
    {
        public PeDataDirectory(byte* memory, ulong offset) : base(memory, offset)
        {
        }

        public uint VirtualAddress => _native->VirtualAddress;
        public uint Size => _native->Size;
    }
}
