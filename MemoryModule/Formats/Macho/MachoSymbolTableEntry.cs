using MemoryModule.Formats.Macho.Natives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    unsafe class MachoSymbolTableEntry
    {
        private byte* _memory;
        private byte* _data;

        private MachoSymbolTableEntryNative* Entry => (MachoSymbolTableEntryNative*)_data;

        public MachoSymbolTableEntry(byte* memory, ulong offset)
        {
            _memory = memory;
            _data = _memory + offset;
        }

        public ulong Value => (ulong)Entry->n_value;
        public uint StringTableIndex => Entry->n_un.n_strx;
    }
}
