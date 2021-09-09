namespace MemoryModule.Linux.Elf
{
    internal enum ElfArchitecture : ushort
    {
        None = 0x00,
        x86 = 0x03,
        ARM = 0x28,
        IA_64 = 0x32,
        x86_64 = 0x3E,
        ARM_64 = 0xB7
    }
}