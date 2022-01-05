namespace MemoryModule.Formats.Elf
{
    public enum ElfProgramHeaderType : uint
    {
        Null = 0x0,
        Load = 0x1,
        Dynamic = 0x2,
        Interpreter = 0x3,
        Note = 0x4,
        ShLib = 0x5,
        ProgramHeaderTable = 0x6,
        ThreadLocalStorage = 0x7,
        LoOS = 0x60000000,
        HiOS = 0x6FFFFFFF,
        LoProc = 0x70000000,
        HiProc = 0x7FFFFFFF,
    }
}