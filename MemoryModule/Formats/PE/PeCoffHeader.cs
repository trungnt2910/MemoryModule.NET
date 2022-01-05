namespace MemoryModule.Formats.PE
{
    unsafe class PeCoffHeader : MemoryValueObject<PeCoffHeaderNative>
    {
        public PeCoffHeader(byte* memory, ulong offset) : base(memory, offset)
        {
        }

        public PeMachineType MachineType => _native->Machine;
        public ushort SizeOfOptionalHeader => _native->SizeOfOptionalHeader;
        public ushort NumberOfSections => _native->NumberOfSections;
    }
}