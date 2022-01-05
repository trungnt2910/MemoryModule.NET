using MemoryModule.Abstractions;
using MemoryModule.Formats.Macho.Natives;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace MemoryModule.Formats.Macho
{
    unsafe class MachoSegmentLoadCommand : MachoLoadCommand, ISection
    {
        private MachoSegmentLoadCommandNative* Command => (MachoSegmentLoadCommandNative*)_data;
        private string _name;

        private MachoSectionArray _sections;

        public MachoSegmentLoadCommand(byte* memory, ulong fileOffset) : base(memory, fileOffset)
        {
            Debug.Assert(CommandType == (Environment.Is64BitProcess ? MachoLoadCommandType.LC_SEGMENT_64 : MachoLoadCommandType.LC_SEGMENT));

            var offset = Marshal.OffsetOf<MachoSegmentLoadCommandNative>(nameof(MachoSegmentLoadCommandNative.segname));
            _name = Marshal.PtrToStringAnsi((IntPtr)(_data + (ulong)offset));

            _sections = new MachoSectionArray(_memory, fileOffset + (ulong)sizeof(MachoSegmentLoadCommandNative), Command->nsects);
        }

        public string Name => _name;

        public MachoSectionArray Sections => _sections;

        [Obsolete("Use MemoryOffset instead.")]
        public ulong VirtualAddress => MemoryOffset;
        [Obsolete("Use FileOffset instead.")]
        public ulong Offset => FileOffset;

        public MachoVmProtection InitialProtection => Command->initprot;
        public MachoVmProtection MaxProtection => Command->maxprot;

        public ulong MemoryOffset => (ulong)Command->vmaddr;
        public ulong MemorySize => (ulong)Command->vmsize;

        public ulong FileOffset => (ulong)Command->fileoff;
        public ulong FileSize => (ulong)Command->filesize;

        public MemoryProtection MemoryProtection => ConvertProtection(Command->initprot);

        public SectionType Type => SectionType.Unknown;

        private static MemoryProtection ConvertProtection(MachoVmProtection protection)
        {
            var result = (MemoryProtection)0;

            if (protection.HasFlag(MachoVmProtection.Read))
            {
                result |= MemoryProtection.Read;
            }

            if (protection.HasFlag(MachoVmProtection.Write))
            {
                result |= MemoryProtection.Write;
            }

            if (protection.HasFlag(MachoVmProtection.Execute))
            {
                result |= MemoryProtection.Execute;
            }

            return result;
        }

        public override string ToString()
        {
            return
$@"{base.ToString()},
Name:            {_name},
Memory offset:   0x{(ulong)Command->vmaddr:x},
Memory size:     0x{(ulong)Command->vmsize:x},
File offset:     0x{(ulong)Command->fileoff:x},
File size:       0x{(ulong)Command->filesize:x},
Max protection:  0x{(ulong)Command->maxprot:x},
Init protection: 0x{(ulong)Command->initprot:x},
Section count:   {Command->nsects},
Sections:
{string.Join("\n", Sections.Select(sec => "    " + sec.ToString().Replace("\n", "\n    ")))}";
        }
    }
}