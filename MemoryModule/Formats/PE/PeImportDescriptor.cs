using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeImportDescriptor : MemoryValueObject<PeImportDescriptorNative>
    {
        public PeImportDescriptor(byte* memory, ulong offset) : base(memory, offset)
        {
            if (_native->Name != 0)
            {
                Name = Marshal.PtrToStringAnsi((IntPtr)GetAddress(_native->Name));
            }
        }

        public string Name { get; private set; }
        public ulong OriginalFirstThunk => _native->OriginalFirstThunk;
        public ulong FirstThunk => _native->FirstThunk;
    }
}
