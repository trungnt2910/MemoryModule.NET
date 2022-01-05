using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class DosHeader : MemoryValueObject<DosHeaderNative>
    {
        public DosHeader(byte* memory, ulong offset = 0) : base(memory, offset)
        {
        }
        
        /// <summary>
        /// The offset to the new PE header.
        /// </summary>
        public ulong NewHeaderOffset => (ulong)_native->e_lfanew;
    }
}
