using MemoryModule.Abstractions;
using MemoryModule.Formats.Elf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    unsafe class ElfProgramHeader : ISection
    {
        private byte* memory;
        private ElfProgramHeaderNative* obj;

        public ElfProgramHeaderType Type
        {
            get => (Environment.Is64BitProcess) ? obj->Data64.Type : obj->Data32.Type;
        }

        [Obsolete("Use FileOffset instead")]
        public ulong Offset => FileOffset;

        [Obsolete("Use MemoryOffset instead")]
        public ulong VirtualAddress => MemoryOffset;

        public ulong Align
        {
            get => Environment.Is64BitProcess ? (ulong)obj->Data64.Align : obj->Data32.Align;
        }

        public ElfProgramHeaderLoadFlags Flags => (ElfProgramHeaderLoadFlags)(Environment.Is64BitProcess ? obj->Data64.Flags : obj->Data32.Flags);

        #region ISection
        public ulong MemoryOffset => Environment.Is64BitProcess ? (ulong)obj->Data64.VirtualAddress : obj->Data32.VirtualAddress;

        public ulong MemorySize => Environment.Is64BitProcess ? (ulong)obj->Data64.MemorySize : obj->Data32.MemorySize;

        public ulong FileOffset => Environment.Is64BitProcess ? (ulong)obj->Data64.Offset : obj->Data32.Offset;

        public ulong FileSize => Environment.Is64BitProcess ? (ulong)obj->Data64.FileSize : obj->Data32.FileSize;

        public MemoryProtection MemoryProtection => ElfLoadFlagsToMemoryProtection(Flags);

        public string Name => null;

        SectionType ISection.Type => SectionType.Unknown;
        #endregion

        public ElfProgramHeader(byte* memory, ElfProgramHeaderNative* obj)
        {
            this.memory = memory;
            this.obj = obj;
        }

        private MemoryProtection ElfLoadFlagsToMemoryProtection(ElfProgramHeaderLoadFlags flags)
        {
            var result = (MemoryProtection)0;

            if (flags.HasFlag(ElfProgramHeaderLoadFlags.Execute))
            {
                result |= MemoryProtection.Execute;
            }

            if (flags.HasFlag(ElfProgramHeaderLoadFlags.Read))
            {
                result |= MemoryProtection.Read;
            }

            if (flags.HasFlag(ElfProgramHeaderLoadFlags.Write))
            {
                result |= MemoryProtection.Write;
            }

            return result;
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
