using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.MacOS.Macho.Natives
{
    [StructLayout(LayoutKind.Sequential)]
    struct MachoSegmentLoadCommandNative
    {
        public uint cmd;                        /* LC_SEGMENT_64 */
        public uint cmdsize;                    /* includes sizeof section_64 structs */
        public unsafe fixed byte segname[16];   /* segment name */
        public UIntPtr vmaddr;                  /* memory address of this segment */
        public UIntPtr vmsize;                  /* memory size of this segment */
        public UIntPtr fileoff;                 /* file offset of this segment */
        public UIntPtr filesize;                /* amount to map from the file */
        public uint maxprot;                    /* maximum VM protection */
        public uint initprot;                   /* initial VM protection */
        public uint nsects;                     /* number of sections in segment */
        public uint flags;                      /* flags */
    }
}
