using MemoryModule.Formats.Elf;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    internal unsafe class ElfSectionHeader
    {
        private byte* memory;
        private ElfSectionHeaderNative* obj;
        private string name;

        public ElfSectionHeaderType Type => obj->Type;

        public readonly ElfDynamicSectionArray DynamicSectionItems;

        public ulong Offset => (ulong)obj->Offset;
        public UIntPtr Pointer => (UIntPtr)obj;
        public ulong Size => (ulong)obj->Size;
        public ulong EntrySize => (ulong)obj->EntrySize;

        public ElfSectionHeader(byte* memory, ElfSectionHeaderNative* obj, byte* namePtr = null)
        {
            this.memory = memory;
            this.obj = obj;
            if (namePtr != null)
            {
                name = Marshal.PtrToStringAnsi((IntPtr)namePtr);
            }

            switch (obj->Type)
            {
                case ElfSectionHeaderType.Dynamic:
                    DynamicSectionItems = new ElfDynamicSectionArray(memory, (ulong)obj->Offset);
                break;
            }
        }

        public byte* GetData()
        {
            return memory + (ulong)obj->Offset;
        }

        public override string ToString()
        {
// Dump in a random order, as I don't know what the fuck's happening.
            return 
$@"ELF Section Header at: 0x{(ulong)obj:x}, owned by 0x{(ulong)memory:x}
- Type: {obj->Type}
- Name: {name}
- Info: {obj->Info}
- Offset: 0x{(ulong)obj->Offset:x}
- EntrySize: {obj->EntrySize}
- Flags: {obj->Flags}
- Alignment: {obj->Alignment}
- Virtual Address: 0x{(ulong)obj->VirtualAddress:x}
- Memory Size: {obj->Size}
- Link: {obj->Link}
";
        }
    }
}
