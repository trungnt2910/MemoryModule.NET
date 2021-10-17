using MemoryModule.MacOS.Macho.Natives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    unsafe class MachoLoadCommand
    {
        private MachoLoadCommandNative* Command => (MachoLoadCommandNative *)_data;

        protected byte* _memory;
        protected byte* _data;

        protected MachoLoadCommand(byte* memory, ulong offset)
        {
            _memory = memory;
            _data = _memory + offset;
        }

        public MachoLoadCommandType CommandType => Command->cmd;
        public uint CommandSize => Command->cmdsize;

        public static MachoLoadCommand Construct(byte* data, ulong offset)
        {
            var _command = (MachoLoadCommandNative*)(data + offset);

            switch (_command->cmd)
            {
                case MachoLoadCommandType.LC_SEGMENT:
                case MachoLoadCommandType.LC_SEGMENT_64:
                    return new MachoSegmentLoadCommand(data, offset);
                case MachoLoadCommandType.LC_DYLD_INFO_ONLY:
                    return new MachoDyldInfoOnlyLoadCommand(data, offset);
                case MachoLoadCommandType.LC_ID_DYLIB:
                case MachoLoadCommandType.LC_LOAD_DYLIB:
                    return new MachoDylibLoadCommand(data, offset);
                case MachoLoadCommandType.LC_SYMTAB:
                    return new MachoSymbolTableLoadCommand(data, offset);
                default:
                    return new MachoLoadCommand(data, offset);
            }
        }

        public override string ToString()
        {
            return
$@"Command Type: {Command->cmd},
Command Size: {Command->cmdsize}";
        }
    }
}
