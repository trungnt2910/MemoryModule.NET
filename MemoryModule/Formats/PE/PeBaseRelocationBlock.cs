using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeBaseRelocationBlock : MemoryValueObject<PeBaseRelocationBlockNative>, IReadOnlyList<PeBaseRelocation>
    {
        private class PeBaseRelocationBlockInternal : MemoryValueObjectArray<ushort, PeBaseRelocation>
        {
            private ulong _parentRva;

            public PeBaseRelocationBlockInternal(byte* memory, ulong offset, ulong size, ulong parentRva) : base(memory, offset, size)
            {
                _parentRva = parentRva;
            }

            protected override unsafe PeBaseRelocation Construct(byte* memory, ulong offset)
            {
                return new PeBaseRelocation(memory, offset, _parentRva);
            }
        }

        private readonly PeBaseRelocationBlockInternal _list;

        public PeBaseRelocationBlock(byte* memory, ulong offset) : base(memory, offset)
        {
            _list = new PeBaseRelocationBlockInternal(memory, offset + (uint)sizeof(PeBaseRelocationBlockNative), (_native->SizeOfBlock - (uint)sizeof(PeBaseRelocationBlockNative)) / sizeof(ushort), _native->VirtualAddress);
        }

        public int Count => ((IReadOnlyCollection<PeBaseRelocation>)_list).Count;

        public PeBaseRelocation this[int index] => ((IReadOnlyList<PeBaseRelocation>)_list)[index];

        public IEnumerator<PeBaseRelocation> GetEnumerator()
        {
            return ((IEnumerable<PeBaseRelocation>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}
