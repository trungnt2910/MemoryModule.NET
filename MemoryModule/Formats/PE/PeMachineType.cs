namespace MemoryModule.Formats.PE
{
    public enum PeMachineType : ushort
    {
        /// <summary>
        /// The content of this field is assumed to be applicable to any machine type
        /// </summary>
        Unknown = 0x0,
        /// <summary>
        /// Matsushita AM33
        /// </summary>
        Am33 = 0x1d3,
        /// <summary>
        /// x64
        /// </summary>
        Amd64 = 0x8664,
        /// <summary>
        /// ARM little endian
        /// </summary>
        Arm = 0x1c0,
        /// <summary>
        /// ARM64 little endian
        /// </summary>
        Arm64 = 0xaa64,
        /// <summary>
        /// ARM Thumb-2 little endian
        /// </summary>
        Armnt = 0x1c4,
        /// <summary>
        /// EFI byte code
        /// </summary>
        Ebc = 0xebc,
        /// <summary>
        /// Intel 386 or later processors and compatible processors
        /// </summary>
        I386 = 0x14c,
        /// <summary>
        /// Intel Itanium processor family
        /// </summary>
        Ia64 = 0x200,
        /// <summary>
        /// Mitsubishi M32R little endian
        /// </summary>
        M32r = 0x9041,
        /// <summary>
        /// MIPS16
        /// </summary>
        Mips16 = 0x266,
        /// <summary>
        /// MIPS with FPU
        /// </summary>
        Mipsfpu = 0x366,
        /// <summary>
        /// MIPS16 with FPU
        /// </summary>
        Mipsfpu16 = 0x466,
        /// <summary>
        /// Power PC little endian
        /// </summary>
        Powerpc = 0x1f0,
        /// <summary>
        /// Power PC with floating point support
        /// </summary>
        Powerpcfp = 0x1f1,
        /// <summary>
        /// MIPS little endian
        /// </summary>
        R4000 = 0x166,
        /// <summary>
        /// RISC-V 32-bit address space
        /// </summary>
        Riscv32 = 0x5032,
        /// <summary>
        /// RISC-V 64-bit address space
        /// </summary>
        Riscv64 = 0x5064,
        /// <summary>
        /// RISC-V 128-bit address space
        /// </summary>
        Riscv128 = 0x5128,
        /// <summary>
        /// Hitachi SH3
        /// </summary>
        Sh3 = 0x1a2,
        /// <summary>
        /// Hitachi SH3 DSP
        /// </summary>
        Sh3dsp = 0x1a3,
        /// <summary>
        /// Hitachi SH4
        /// </summary>
        Sh4 = 0x1a6,
        /// <summary>
        /// Hitachi SH5
        /// </summary>
        Sh5 = 0x1a8,
        /// <summary>
        /// Thumb
        /// </summary>
        Thumb = 0x1c2,
        /// <summary>
        /// MIPS little-endian WCE v2
        /// </summary>
        Wcemipsv2 = 0x169,
    }
}