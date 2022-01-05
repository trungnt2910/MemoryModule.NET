namespace MemoryModule.Formats.Elf
{
    internal enum ElfObjectType : ushort
    {
        None = 0x00,
        Rel = 0x01,
        Exec = 0x02,
        Dyn = 0x03,
        Core = 0x04,
        LoOS = 0xFE00,
        HiOS = 0xFEFF,
        LoProc = 0xFF00,
        HiProc = 0xFFFF
    }
}