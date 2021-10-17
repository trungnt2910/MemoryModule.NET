using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryModule.MacOS.Macho.Natives
{
    [StructLayout(LayoutKind.Explicit)]
    struct MachoHeaderNative
    {
        [FieldOffset(0)]
        public MachoMagic magic;                          /* mach magic number identifier */
        [FieldOffset(4)]
        public MachoCpuType cputype;                        /* cpu specifier */
        [FieldOffset(8)]
        public MachoCpuSubtype cpusubtype;                     /* machine specifier */
        [FieldOffset(12)]
        public MachoFileType filetype;                       /* type of file */
        [FieldOffset(16)]
        public uint ncmds;                          /* number of load commands */
        [FieldOffset(20)]
        public uint sizeofcmds;                     /* the size of all the load commands */
        [FieldOffset(24)]
        public UIntPtr flags_and_reserved;          /* A bundle that is 64-bit on 64-bit platforms. */
        [FieldOffset(24)]
        public MachoFileFlags flags;                          /* flags */

        public uint reserved
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (uint)((ulong)flags_and_reserved & uint.MaxValue);
        }
    }
}
