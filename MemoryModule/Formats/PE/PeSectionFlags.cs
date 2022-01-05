using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    [Flags]
    enum PeSectionFlags : uint
    {
        /// <summary>
        /// The section should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid only for object files.
        /// </summary>
        TypeNoPad = 0x00000008,
        /// <summary>
        /// The section contains executable code.
        /// </summary>
        CntCode = 0x00000020,
        /// <summary>
        /// The section contains initialized data.
        /// </summary>
        CntInitializedData = 0x00000040,
        /// <summary>
        /// The section contains uninitialized data.
        /// </summary>
        CntUninitializedData = 0x00000080,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        LnkOther = 0x00000100,
        /// <summary>
        /// The section contains comments or other information. The .drectve section has this type. This is valid for object files only.
        /// </summary>
        LnkInfo = 0x00000200,
        /// <summary>
        /// The section will not become part of the image. This is valid only for object files.
        /// </summary>
        LnkRemove = 0x00000800,
        /// <summary>
        /// The section contains COMDAT data. For more information, see COMDAT Sections (Object Only). This is valid only for object files.
        /// </summary>
        LnkComdat = 0x00001000,
        /// <summary>
        /// The section contains data referenced through the global pointer (GP).
        /// </summary>
        Gprel = 0x00008000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemPurgeable = 0x00020000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        Mem16Bit = 0x00020000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemLocked = 0x00040000,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        MemPreload = 0x00080000,
        /// <summary>
        /// Align data on a 1-byte boundary. Valid only for object files.
        /// </summary>
        Align1Bytes = 0x00100000,
        /// <summary>
        /// Align data on a 2-byte boundary. Valid only for object files.
        /// </summary>
        Align2Bytes = 0x00200000,
        /// <summary>
        /// Align data on a 4-byte boundary. Valid only for object files.
        /// </summary>
        Align4Bytes = 0x00300000,
        /// <summary>
        /// Align data on a 8-byte boundary. Valid only for object files.
        /// </summary>
        Align8Bytes = 0x00400000,
        /// <summary>
        /// Align data on a 16-byte boundary. Valid only for object files.
        /// </summary>
        Align16Bytes = 0x00500000,
        /// <summary>
        /// Align data on a 32-byte boundary. Valid only for object files.
        /// </summary>
        Align32Bytes = 0x00600000,
        /// <summary>
        /// Align data on a 64-byte boundary. Valid only for object files.
        /// </summary>
        Align64Bytes = 0x00700000,
        /// <summary>
        /// Align data on a 128-byte boundary. Valid only for object files.
        /// </summary>
        Align128Bytes = 0x00800000,
        /// <summary>
        /// Align data on a 256-byte boundary. Valid only for object files.
        /// </summary>
        Align256Bytes = 0x00900000,
        /// <summary>
        /// Align data on a 512-byte boundary. Valid only for object files.
        /// </summary>
        Align512Bytes = 0x00A00000,
        /// <summary>
        /// Align data on a 1024-byte boundary. Valid only for object files.
        /// </summary>
        Align1024Bytes = 0x00B00000,
        /// <summary>
        /// Align data on a 2048-byte boundary. Valid only for object files.
        /// </summary>
        Align2048Bytes = 0x00C00000,
        /// <summary>
        /// Align data on a 4096-byte boundary. Valid only for object files.
        /// </summary>
        Align4096Bytes = 0x00D00000,
        /// <summary>
        /// Align data on a 8192-byte boundary. Valid only for object files.
        /// </summary>
        Align8192Bytes = 0x00E00000,
        /// <summary>
        /// Mask to get Align value.
        /// </summary>
        AlignMask = 0x00F00000,
        /// <summary>
        /// The section contains extended relocations.
        /// </summary>
        LnkNrelocOvfl = 0x01000000,
        /// <summary>
        /// The section can be discarded as needed.
        /// </summary>
        MemDiscardable = 0x02000000,
        /// <summary>
        /// The section cannot be cached.
        /// </summary>
        MemNotCached = 0x04000000,
        /// <summary>
        /// The section is not pageable.
        /// </summary>
        MemNotPaged = 0x08000000,
        /// <summary>
        /// The section can be shared in memory.
        /// </summary>
        MemShared = 0x10000000,
        /// <summary>
        /// The section can be executed as code.
        /// </summary>
        MemExecute = 0x20000000,
        /// <summary>
        /// The section can be read.
        /// </summary>
        MemRead = 0x40000000,
        /// <summary>
        /// The section can be written to.
        /// </summary>
        MemWrite = 0x80000000,
    }
}
