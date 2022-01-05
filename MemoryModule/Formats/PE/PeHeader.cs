using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeHeader : MemoryValueObject<PeHeaderNative>
    {
        public uint Magic => _native->Signature;
        public PeCoffHeader FileHeader => _coff;
        public PeOptionalHeader PeOptionalHeader => _pe;

        private readonly PeCoffHeader _coff;
        private readonly PeOptionalHeader _pe;

        public PeHeader(byte* memory, ulong offset) : base(memory, offset)
        {
            _coff = new PeCoffHeader(memory, offset + (ulong)Marshal.OffsetOf<PeHeaderNative>(nameof(PeHeaderNative.FileHeader)));
            _pe = new PeOptionalHeader(memory, offset + (ulong)Marshal.OffsetOf<PeHeaderNative>(nameof(PeHeaderNative.OptionalHeader)));
        }
    }
}
