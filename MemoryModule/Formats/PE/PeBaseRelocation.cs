using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeBaseRelocation : MemoryValueObject<ushort>, IRebase
    {
        public PeBaseRelocationType Type { get; private set; }

        public PeBaseRelocation(byte* memory, ulong offset, ulong parentRva) : base(memory, offset)
        {
            var value = *(ushort*)(memory + offset);
            AffectedAddress = parentRva + (value & 0xfffu);
            Type = (PeBaseRelocationType)(value >> 12);

            switch (Type)
            {
                case PeBaseRelocationType.Absolute:
                case PeBaseRelocationType.Highlow:
                case PeBaseRelocationType.Highadj:
                case PeBaseRelocationType.Dir64:
                    break;
                default:
                    throw new NotImplementedException($"Unsupported relocation type: {Type}");
            }
        }

        public ulong AffectedAddress { get; private set; }

        public ulong Addend => 0;

        public bool IgnoreExistingValue => false;
    }
}
