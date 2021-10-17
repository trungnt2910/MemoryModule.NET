using MemoryModule.MacOS.Macho.Natives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    unsafe class MachoSymbolTableLoadCommand : MachoLoadCommand
    {
        private MachoSymbolTableLoadCommandNative* Command => (MachoSymbolTableLoadCommandNative*)_data;

        public MachoSymbolTableLoadCommand(byte* memory, ulong offset) : base(memory, offset)
        {
        }

        public uint SymbolTableOffset => Command->symoff;
        public uint SymbolCount => Command->nsyms;

        public uint StringTableOffset => Command->stroff;
        public uint StringTableSize => Command->strsize;

        public override string ToString()
        {
            return
$@"{base.ToString()}
Symbol table offset: 0x{SymbolTableOffset:x}
Symbol count:        {SymbolCount}
String table offset: 0x{StringTableOffset:x}
String table size:   {StringTableSize}";
        }
    }
}
