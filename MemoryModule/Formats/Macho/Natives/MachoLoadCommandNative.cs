using System.Runtime.InteropServices;

namespace MemoryModule.Formats.Macho.Natives
{
    [StructLayout(LayoutKind.Sequential)]
    struct MachoLoadCommandNative
    {
        public MachoLoadCommandType cmd;           /* type of load command */
        public uint cmdsize;                       /* total size of command in bytes */
    }
}