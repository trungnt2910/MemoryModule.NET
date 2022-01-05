using MemoryModule.Formats.Elf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    internal unsafe class ElfDynamicSectionItem
    {
        private byte* _memory;
        private ElfDynamicSectionItemNative* _obj;

        public ElfDynamicSectionItemType Tag => (ElfDynamicSectionItemType)_obj->Tag;
        public UIntPtr Pointer => _obj->ValueOrPtr.Ptr;
        public ulong Value => (ulong)_obj->ValueOrPtr.Value;

        public ElfDynamicSectionItem(byte* memory, ElfDynamicSectionItemNative* ptr)
        {
            _memory = memory;
            _obj = ptr;
        }

        public override string ToString()
        {
            return
$@"ELF Dynamic Section:
- Tag: {Tag}
- Value/Pointer: 0x{Value:x}
";
        }
    }
}
