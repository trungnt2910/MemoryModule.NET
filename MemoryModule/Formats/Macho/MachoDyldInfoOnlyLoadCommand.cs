using MemoryModule.Formats.Macho.Natives;
using System.Runtime.InteropServices;

namespace MemoryModule.Formats.Macho
{
    unsafe class MachoDyldInfoOnlyLoadCommand : MachoLoadCommand
    {
        private MachoDyldInfoLoadCommandNative* Command => (MachoDyldInfoLoadCommandNative*)_data;

        public MachoDyldInfoOnlyLoadCommand(byte* memory, ulong fileOffset) : base(memory, fileOffset)
        {
        }

        public ulong RebaseOffset => Command->rebase_off;
        public ulong RebaseSize => Command->rebase_size;

        public ulong BindOffset => Command->bind_off;
        public ulong BindSize => Command->bind_size;

        public ulong WeakBindOffset => Command->weak_bind_off;
        public ulong WeakBindSize => Command->weak_bind_size;

        public ulong LazyBindOffset => Command->lazy_bind_off;
        public ulong LazyBindSize => Command->lazy_bind_size;

        public ulong ExportOffset => Command->export_off;
        public ulong ExportSize => Command->export_size;

        public override string ToString()
        {
            return
$@"Rebase offset:    {Command->rebase_off},
Rebase size:      {Command->rebase_size},
Bind offset:      {Command->bind_off},
Bind size:        {Command->bind_size},
Weak bind offset: {Command->weak_bind_off},
Weak bind size:   {Command->weak_bind_size},
Lazy bind offset: {Command->lazy_bind_off},
Lazy bind size:   {Command->lazy_bind_size},
Export offset:    {Command->export_off},
Export size:      {Command->export_size}";
        }
    }
}