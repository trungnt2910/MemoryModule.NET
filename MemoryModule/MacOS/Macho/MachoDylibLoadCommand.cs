using MemoryModule.MacOS.Macho.Natives;
using System;
using System.Runtime.InteropServices;

namespace MemoryModule.MacOS.Macho
{
    unsafe class MachoDylibLoadCommand : MachoLoadCommand
    {
        private MachoDylibLoadCommandNative* Command => (MachoDylibLoadCommandNative*)_data;
        private string _name;

        public MachoDylibLoadCommand(byte* memory, ulong offset) : base(memory, offset)
        {
            _name = Marshal.PtrToStringAnsi((IntPtr)(_data + Command->name));
        }

        public string Name => _name;
        public byte* NamePtr => _data + Command->name;

        public override string ToString()
        {
            return
$@"{base.ToString()},
{((CommandType == MachoLoadCommandType.LC_ID_DYLIB) ? 
$"Library name: {Name}" :
$"Requires:     {Name}")}";
        }
    }
}