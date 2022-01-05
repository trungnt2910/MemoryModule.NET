using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeExportDirectory : MemoryValueObject<PeExportDirectoryNative>
    {
        public PeExportDirectory(byte* memory, ulong offset) : base(memory, offset)
        {
            Name = Marshal.PtrToStringAnsi((IntPtr)GetAddress(_native->Name));
        }

        public string Name { get; private set; }
        public uint OrdinalBase => _native->Base;
        public uint EntryCount => _native->NumberOfFunctions;
        public uint NameCount => _native->NumberOfNames;
        public uint NameArrayOffset => _native->AddressOfNames;
        public uint AddressArrayOffset => _native->AddressOfFunctions;
        public uint OrdinalArrayOffset => _native->AddressOfNameOrdinals;
    }
}
