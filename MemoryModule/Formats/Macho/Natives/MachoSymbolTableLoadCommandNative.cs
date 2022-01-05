using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Macho.Natives
{
    [StructLayout(LayoutKind.Sequential)]
    struct MachoSymbolTableLoadCommandNative
    {
        public uint cmd;            /* LC_SYMTAB */
        public uint cmdsize;        /* sizeof(struct symtab_command) */
        public uint symoff;         /* symbol table offset */
        public uint nsyms;          /* number of symbol table entries */
        public uint stroff;         /* string table offset */
        public uint strsize;        /* string table size in bytes */
    }
}
