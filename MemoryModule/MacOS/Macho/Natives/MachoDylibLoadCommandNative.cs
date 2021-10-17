using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.MacOS.Macho.Natives
{
    [StructLayout(LayoutKind.Sequential)]
    struct MachoDylibLoadCommandNative
    {
        public MachoLoadCommandType cmd;           /* type of load command */
        public uint cmdsize;                       /* total size of command in bytes */

        public uint name;              // Offset to the name
        public uint timestamp;
        public uint current_version;
        public uint compatibility_version;
    }
}
