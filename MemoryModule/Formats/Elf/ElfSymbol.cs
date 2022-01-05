using MemoryModule.Abstractions;
using MemoryModule.Formats.Elf;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    internal unsafe class ElfSymbol : ISymbol
    {
        private byte* _memory;
        private ElfSymbolNative* _obj;
        private string _name;

        public ElfSymbol(byte* memory, ElfSymbolNative* obj)
        {
            _memory = memory;
            _obj = obj;
        }

        public ElfSymbolType Type
        {
            get
            {
                var info = Environment.Is64BitProcess ? _obj->Data64.Info : _obj->Data32.Info;
                return (ElfSymbolType)(info & 0xf);
            }
        }

        public ElfSymbolBinding Binding
        {
            get
            {
                var info = Environment.Is64BitProcess ? _obj->Data64.Info : _obj->Data32.Info;
                return (ElfSymbolBinding)(info >> 4);
            }
        }

        public string Name => _name;

        public ushort SectionHeaderTableIndex
        {
            get => Environment.Is64BitProcess ? _obj->Data64.SectionHeaderTableIndex : _obj->Data32.SectionHeaderTableIndex;
        }

        public ushort Other => Environment.Is64BitProcess ? _obj->Data64.Other : _obj->Data32.Other;

        public ulong Value
        {
            get => Environment.Is64BitProcess ? (ulong)_obj->Data64.Value : _obj->Data32.Value;
            set
            {
                if (Environment.Is64BitProcess)
                {
                    _obj->Data64.Value = (UIntPtr)value;
                }
                else
                {
                    _obj->Data32.Value = (uint)value;
                }

            }
        }

        public ulong Size => Environment.Is64BitProcess ? (ulong)_obj->Data64.Size : _obj->Data32.Size;

        public IntPtr Address { get; internal set; }

        public override string ToString()
        {
            return
$@"ELF Symbol:
- Name: {Name}
- Type: {Type}
- Binding: {Binding}
- Other: {Other}
- Value: 0x{Value:x}
- Size: {Size}
- Index: {SectionHeaderTableIndex}
";
        }

        /// <summary>
        /// Resolves the symbols name, using values from the specified
        /// symbol table. Subsequent calls to Name will return this value.
        /// </summary>
        /// <param name="stringTable"></param>
        /// <returns></returns>
        public string ResolveName(ElfStringTable stringTable)
        {
            return _name = stringTable.GetString(Environment.Is64BitProcess ? _obj->Data64.Name : _obj->Data32.Name);
        }
    }
}
