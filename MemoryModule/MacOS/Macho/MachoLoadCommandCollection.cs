using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho.Natives
{
    unsafe class MachoLoadCommandCollection : IReadOnlyCollection<MachoLoadCommand>
    {
        private int _count;
        private ulong _arrOffset;
        private byte* _memory;
        private byte* _data;

        public MachoLoadCommandCollection(byte* data, ulong offset, uint count)
        {
            _count = (int)count;
            _memory = data;
            _data = data + offset;
            _arrOffset = offset;
        }

        public int Count => _count;

        public IEnumerator<MachoLoadCommand> GetEnumerator()
        {
            uint offset = 0;

            for (int i = 0; i < _count; ++i)
            {
                yield return Construct(ref offset);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private MachoLoadCommand Construct(ref uint offset)
        {
            var command = MachoLoadCommand.Construct(_memory, _arrOffset + offset);
            offset += command.CommandSize;
            return command;
        }
    }
}
