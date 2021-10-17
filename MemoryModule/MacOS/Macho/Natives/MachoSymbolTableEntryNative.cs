using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.MacOS.Macho.Natives
{
    [StructLayout(LayoutKind.Sequential)]
    struct MachoSymbolTableEntryNative
    {
        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct DummyUnion
        {
            [FieldOffset(0)]
            private uint n_name_field;
            [FieldOffset(0)]
            public uint n_strx;
            public byte* n_name => (byte*)n_name_field;
        }
        public DummyUnion n_un;
        public byte n_type;             /* type flag, see below */
        public byte n_sect;             /* section number or NO_SECT */
        public short n_desc;            /* see <mach-o/stab.h> */
        public UIntPtr n_value;         /* value of this symbol (or stab offset) */
    }
}
