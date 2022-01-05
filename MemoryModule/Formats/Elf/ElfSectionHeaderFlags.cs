using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    [Flags]
    public enum ElfSectionHeaderFlags : ulong
    {
//0x1	SHF_WRITE   Writable
//0x2	SHF_ALLOC   Occupies memory during execution
//0x4	SHF_EXECINSTR   Executable
//0x10	SHF_MERGE   Might be merged
//0x20	SHF_STRINGS Contains null-terminated strings
//0x40	SHF_INFO_LINK	'sh_info' contains SHT index
//0x80	SHF_LINK_ORDER  Preserve order after combining
//0x100	SHF_OS_NONCONFORMING    Non-standard OS specific handling required
//0x200	SHF_GROUP   Section is member of a group
//0x400	SHF_TLS Section hold thread-local data
//0x0ff00000	SHF_MASKOS  OS-specific
//0xf0000000	SHF_MASKPROC    Processor-specific
//0x4000000	SHF_ORDERED Special ordering requirement (Solaris)
//0x8000000	SHF_EXCLUDE Section is excluded unless referenced or allocated (Solaris)
    
        Writeable = 0x1,
        Alloc = 0x2,
        Executable = 0x4,
        Merge = 0x10,
        Strings = 0x20,
        InfoLink = 0x40,
        LinkOrder = 0x80,
        OsNonconforming = 0x100,
        Group = 0x200,
        ThreadLocalStorage = 0x400,
        MaskOs = 0x0ff00000,
        MaskProc = 0xf0000000
    }
}
