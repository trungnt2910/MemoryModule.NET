namespace MemoryModule.Linux.Elf
{
    public enum ElfSectionHeaderType : uint
    {
//0x0	SHT_NULL    Section header table entry unused
//0x1	SHT_PROGBITS    Program data
//0x2	SHT_SYMTAB  Symbol table
//0x3	SHT_STRTAB  String table
//0x4	SHT_RELA    Relocation entries with addends
//0x5	SHT_HASH    Symbol hash table
//0x6	SHT_DYNAMIC Dynamic linking information
//0x7	SHT_NOTE    Notes
//0x8	SHT_NOBITS  Program space with no data (bss)
//0x9	SHT_REL Relocation entries, no addends
//0x0A	SHT_SHLIB   Reserved
//0x0B	SHT_DYNSYM  Dynamic linker symbol table
//0x0E	SHT_INIT_ARRAY  Array of constructors
//0x0F	SHT_FINI_ARRAY  Array of destructors
//0x10	SHT_PREINIT_ARRAY   Array of pre-constructors
//0x11	SHT_GROUP   Section group
//0x12	SHT_SYMTAB_SHNDX    Extended section indices
//0x13	SHT_NUM Number of defined types.
//0x60000000	SHT_LOOS    Start OS-specific.
        
        Null = 0x0,
        ProgramBits = 0x1,
        SymbolTable = 0x2,
        StringTable = 0x3,
        RelocationAddends = 0x4,
        Hash = 0x5,
        Dynamic = 0x6,
        Note = 0x7,
        NoBits = 0x8,
        Relocation = 0x9,
        ShLib = 0x0A,
        DynamicSymbol = 0x0B,
        InitArray = 0x0E,
        FinalizeArray = 0x0F,
        PreInitArray = 0x10,
        Group = 0x11,
        SymbolTableExtendedIndices = 0x12,
        NumberOfDefindedTypes = 0x13,

        LoOS = 0x60000000,
    }
}