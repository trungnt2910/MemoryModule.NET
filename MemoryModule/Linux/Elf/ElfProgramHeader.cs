using MemoryModule.Linux.Elf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Linux.Elf
{
    public unsafe class ElfProgramHeader
    {
        private byte* memory;
        private ElfProgramHeaderNative* obj;

        public ElfProgramHeaderType Type
        {
            get => (Environment.Is64BitProcess) ? obj->Data64.Type : obj->Data32.Type;
        }

        public ulong Offset
        {
            get => Environment.Is64BitProcess ? (ulong)obj->Data64.Offset : obj->Data32.Offset;
        }

        public ulong FileSize
        {
            get => Environment.Is64BitProcess ? (ulong)obj->Data64.FileSize : obj->Data32.FileSize;
        }
        
        public ulong VirtualAddress
        {
            get => Environment.Is64BitProcess ? (ulong)obj->Data64.VirtualAddress : obj->Data32.VirtualAddress;
        }

        public ulong MemorySize
        {
            get => Environment.Is64BitProcess ? (ulong)obj->Data64.MemorySize : obj->Data32.MemorySize;
        }

        public ulong Align
        {
            get => Environment.Is64BitProcess ? (ulong)obj->Data64.Align : obj->Data32.Align;
        }

        public uint Flags => Environment.Is64BitProcess ? obj->Data64.Flags : obj->Data32.Flags;

        public ElfProgramHeader(byte* memory, ElfProgramHeaderNative* obj)
        {
            this.memory = memory;
            this.obj = obj;
        }

        public override string ToString()
        {
            return Environment.Is64BitProcess ?
$@"ELF Program Header at: 0x{(ulong)obj:x}, owned by 0x{(ulong)memory:x}
- Type: {obj->Data64.Type},
- Size on file: 0x{(ulong)obj->Data64.Offset:x}..0x{(obj->Data64.Offset.ToUInt64() + obj->Data64.FileSize.ToUInt64()):x}, {obj->Data64.FileSize} bytes.
- Virtual address: 0x{(ulong)obj->Data64.VirtualAddress:x}..0x{(obj->Data64.VirtualAddress.ToUInt64() + obj->Data64.MemorySize.ToUInt64()):x}
- Flags: {obj->Data64.Flags}
- Alignment: {obj->Data64.Align}
" :
$@"ELF Program Header at: 0x{(ulong)obj:x}, owned by 0x{(ulong)memory:x}
- Type: {obj->Data32.Type},
- Size on file: 0x{(ulong)obj->Data32.Offset:x}..0x{(obj->Data32.Offset + obj->Data32.FileSize):x}, {obj->Data64.FileSize} bytes.
- Virtual address: 0x{(ulong)obj->Data32.VirtualAddress:x}..0x{(obj->Data32.VirtualAddress + obj->Data32.MemorySize):x}
- Flags: {obj->Data32.Flags}
- Alignment: {obj->Data32.Align}
";
        }
    }
}
