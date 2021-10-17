using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    class MachoExport
    {
        public MachoExportSymbolFlags Flags { get; internal set; }
        public string Name { get; internal set; }
        public ulong Location { get; internal set; }
        public ulong LibraryOrdinal { get; internal set; }
        public string ReexportName { get; internal set; }

        public override string ToString()
        {
            return
$@"Type:           {Flags},
Name:           {Name},
Offset:         0x{Location:x},
LibraryOrdinal: {LibraryOrdinal},
ReexportName:   {ReexportName}";
        }
    }
}
