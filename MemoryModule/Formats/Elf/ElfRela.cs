using MemoryModule.Formats.Elf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    public unsafe class ElfRela
    {
        private byte* _memory;
        private ElfRelaNative* _obj;

        public ulong Offset => (ulong)_obj->Offset;
        public ElfRelocationType Type
        {
            get
            {
                if (Environment.Is64BitProcess)
                {
                    return new ElfRelocationType((ulong)_obj->Info & 0xffffffff);
                }
                else
                {
                    return new ElfRelocationType((ulong)_obj->Info & 0xff);
                }
            }
        }

        public ulong Symbol
        {
            get
            {
                if (Environment.Is64BitProcess)
                {
                    return (ulong)_obj->Info >> 32;
                }
                else
                {
                    return (ulong)_obj->Info >> 8;
                }

            }
        }

        public ulong Addend => (ulong)_obj->Addend;

        public bool DependsOnSymbol()
        {
            return Type.Is16() || Type.Is64() || Type.IsGlobDat() || Type.IsJmpSlot();
        }

        public ElfRela(byte* memory, ElfRelaNative* obj)
        {
            _memory = memory;
            _obj = obj;
        }

        public override string ToString()
        {
            return
$@"ELF Relocation Entry at: 0x{(ulong)_obj:x}
- Offset: 0x{Offset:x}
- Type: {Type}
- Symbol: {Symbol}
- Addend: {Addend}
";
        }
    }
}
