using MemoryModule.Formats.Elf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryModule.Formats.Elf
{
    internal unsafe class ElfHeader
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void InitDelegate(int argc, byte** argv, byte** envp);

        private static readonly byte[] MagicSymbol = { 0x7F, 0x45, 0x4c, 0x46 };

        private byte* _memory;
        private ElfHeaderNative* obj;

        public readonly ElfProgramHeaderArray ProgramHeaders;
        public readonly ElfSectionHeaderArray SectionHeaders;

        public readonly ElfSectionHeader DynamicSectionHeader;

        private readonly Dictionary<string, IntPtr> _dependencyHandles = new Dictionary<string, IntPtr>();

        public ElfArchitecture Architecture => obj->Machine;

        public ElfHeader(byte* ptr)
        {
            _memory = ptr;
            obj = (ElfHeaderNative*)ptr;

            if ((obj->Magic[0] != MagicSymbol[0])
                || (obj->Magic[1] != MagicSymbol[1])
                || (obj->Magic[2] != MagicSymbol[2])
                || (obj->Magic[3] != MagicSymbol[3]))
            {
                throw new InvalidOperationException("Not a valid ELF header.");
            }

            ProgramHeaders = new ElfProgramHeaderArray(ptr, (ulong)obj->ProgramHeaderOffset, obj->ProgramHeaderCount);
            SectionHeaders = new ElfSectionHeaderArray(ptr, (ulong)obj->SectionHeaderOffset, obj->SectionHeaderCount, obj->SectionHeaderStringIndex);

            DynamicSectionHeader = SectionHeaders.FirstOrDefault(header => header.Type == ElfSectionHeaderType.Dynamic);
        }
        
        public byte* GetAddress(ulong Offset = 0)
        {
            return _memory + Offset;
        }
       
        public override string ToString()
        {
            return
$@"ELF Header at: 0x{(ulong)_memory:x}
- Pointer class: {obj->Class}
- Endian: {obj->Endianess}
- ELF Version: {obj->Version}
- ABI: {obj->OsAbi}
- ABI Version: {obj->AbiVersion}
- Object type: {obj->Type}
- Instruction set: {obj->Machine}
- ELF Version: {obj->ElfVersion}
- Entry Point Address: 0x{(ulong)obj->Entry:x}
- Program Header Table Location: 0x{(ulong)obj->ProgramHeaderOffset:x}
- Section Header Table Location: 0x{(ulong)obj->SectionHeaderOffset:x}
- Flags: {obj->ArchitectureDependentFlags}
- Header size: {obj->ElfHeaderSize}
- Program Header Table Entry Size: 0x{(ulong)obj->ProgramHeaderSize:x}
- Program Header Table Entry Count: {obj->ProgramHeaderCount}
- Section Header Table Entry Size: 0x{(ulong)obj->SectionHeaderSize:x}
- Section Header Table Entry Count: {obj->SectionHeaderCount} 
- Section Header Table Name Index: {obj->SectionHeaderStringIndex}
";
        }
    }
}
