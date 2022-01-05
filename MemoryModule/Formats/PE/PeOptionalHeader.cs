using System;
using System.Runtime.InteropServices;

namespace MemoryModule.Formats.PE
{
    unsafe class PeOptionalHeader : MemoryValueObject<PeOptionalHeaderNative>
    {
        private readonly PeDataDirectoryArray _dataDirectory;

        public PeOptionalHeader(byte* memory, ulong offset) : base(memory, offset)
        {
            _dataDirectory = new PeDataDirectoryArray(memory, 
                offset + (ulong)Marshal.OffsetOf<PeOptionalHeaderNative>(nameof(PeOptionalHeaderNative.DataDirectoryBuffer)), 
                _native->NumberOfRvaAndSizes);
        }

        public UIntPtr ImageBase => _native->ImageBase;
        public PeDataDirectoryArray DataDirectory => _dataDirectory;
        public uint EntryPointOffset => _native->AddressOfEntryPoint;
    }
}