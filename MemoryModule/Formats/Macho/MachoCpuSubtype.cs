using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    [StructLayout(LayoutKind.Sequential)]
    struct MachoCpuSubtype
    {
        uint value;

        const uint CPU_SUBTYPE_MASK = 0xff000000;
        const uint CPU_SUBTYPE_LIB64 = 0x80000000;

        enum x86Subtypes
        {
            CPU_SUBTYPE_I386 = 3,
            CPU_SUBTYPE_486 = 4,
            CPU_SUBTYPE_486SX = 132,
            CPU_SUBTYPE_586 = 5,
            CPU_SUBTYPE_PENT = CPU_SUBTYPE_586,
            CPU_SUBTYPE_PENTPRO = 22,
            CPU_SUBTYPE_PENTII_M3 = 54,
            CPU_SUBTYPE_PENTII_M5 = 86,
            CPU_SUBTYPE_PENTIUM_4 = 10,
        }

        enum MC680X0Subtypes
        {
            CPU_SUBTYPE_MC680X0_ALL = 1,
            CPU_SUBTYPE_MC68030 = CPU_SUBTYPE_MC680X0_ALL,
            CPU_SUBTYPE_MC68040 = 2,
            CPU_SUBTYPE_MC68030_ONLY = 3,
        }

        enum x86_64Subtypes
        {
            CPU_SUBTYPE_X86_64_ALL = x86Subtypes.CPU_SUBTYPE_I386,
            CPU_SUBTYPE_X86_64_H = 8,
        }

        enum ARMSubtypes
        {
            CPU_SUBTYPE_ARM_ALL = 0,
            CPU_SUBTYPE_ARM_V4T = 5,
            CPU_SUBTYPE_ARM_V6 = 6,
            CPU_SUBTYPE_ARM_V5TEJ = 7,
            CPU_SUBTYPE_ARM_XSCALE = 8,
            CPU_SUBTYPE_ARM_V7 = 9,
            CPU_SUBTYPE_ARM_V7F = 10,
            CPU_SUBTYPE_ARM_V7S = 11,
            CPU_SUBTYPE_ARM_V7K = 12,
            CPU_SUBTYPE_ARM_V6M = 14,
            CPU_SUBTYPE_ARM_V7M = 15,
            CPU_SUBTYPE_ARM_V7EM = 16,
            CPU_SUBTYPE_ARM_V8 = 13,
        }

        enum ARM64Subtypes
        {
            CPU_SUBTYPE_ARM64_ALL = 0,
            CPU_SUBTYPE_ARM64_V8 = 1,
            CPU_SUBTYPE_ARM64E = 2,
        }

        enum ARM64_32Subtypes
        {
            CPU_SUBTYPE_ARM64_32_V8 = 1,
        }

        enum MC88000Subtypes
        {
            CPU_SUBTYPE_MC88000_ALL = 0,
            CPU_SUBTYPE_MMAX_JPC = CPU_SUBTYPE_MC88000_ALL,
            CPU_SUBTYPE_MC88100 = 1,
            CPU_SUBTYPE_MC88110 = 2,
        }

        enum PowerPCSubtypes
        {
            CPU_SUBTYPE_POWERPC_ALL = 0,
            CPU_SUBTYPE_POWERPC_601 = 1,
            CPU_SUBTYPE_POWERPC_602 = 2,
            CPU_SUBTYPE_POWERPC_603 = 3,
            CPU_SUBTYPE_POWERPC_603E = 4,
            CPU_SUBTYPE_POWERPC_603EV = 5,
            CPU_SUBTYPE_POWERPC_604 = 6,
            CPU_SUBTYPE_POWERPC_604E = 7,
            CPU_SUBTYPE_POWERPC_620 = 8,
            CPU_SUBTYPE_POWERPC_750 = 9,
            CPU_SUBTYPE_POWERPC_7400 = 10,
            CPU_SUBTYPE_POWERPC_7450 = 11,
            CPU_SUBTYPE_POWERPC_970 = 100,
        }

        enum PowerPC64Subtypes
        {
            CPU_SUBTYPE_POWERPC64_ALL = PowerPCSubtypes.CPU_SUBTYPE_POWERPC_ALL,
        }

        public string ToString(MachoCpuType type)
        {
            switch (type)
            {
                case MachoCpuType.MC680X0:
                    return ((MC680X0Subtypes)value).ToString();
                case MachoCpuType.MC88000:
                    return ((MC88000Subtypes)value).ToString();
                case MachoCpuType.I386:
                    return ((x86Subtypes)value).ToString();
                case MachoCpuType.X86_64:
                    return ((x86_64Subtypes)value).ToString();
                case MachoCpuType.ARM:
                    return ((ARMSubtypes)value).ToString();
                case MachoCpuType.ARM64:
                    return ((ARM64Subtypes)value).ToString();
                case MachoCpuType.ARM64_32:
                    return ((ARM64_32Subtypes)value).ToString();
                case MachoCpuType.PowerPC:
                    return ((PowerPCSubtypes)value).ToString();
                case MachoCpuType.PowerPC64:
                    return ((PowerPC64Subtypes)value).ToString();
                default:
                    throw new NotSupportedException($"Unsupported CPU type: {type}");
            }
        }
    }
}
