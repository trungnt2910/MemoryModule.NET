using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.MacOS.Macho.Natives
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct MachoSectionNative
    {
        public fixed byte sectname[16];   /* name of this section */
        public fixed byte segname[16];    /* segment this section goes in */
        public UIntPtr addr;           /* memory address of this section */
        public UIntPtr size;           /* size in bytes of this section */
        public uint offset;         /* file offset of this section */
        public uint align;          /* section alignment (power of 2) */
        public uint reloff;         /* file offset of relocation entries */
        public uint nreloc;         /* number of relocation entries */
        public uint flags;          /* flags (section type and attributes)*/
        public uint reserved1;      /* reserved (for offset or index) */
        public UIntPtr reserved2_3;      /* reserved (for count or sizeof) */
    }
}
