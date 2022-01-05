using MemoryModule.Formats.Macho.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    unsafe delegate void InitFunc(int argc, byte** argv, byte** envp, byte** apple);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate void FiniFunc();

    unsafe class MachoHeader
    {
        private static readonly void* RtldDefault = (void*)-2;
        private readonly Dictionary<string, IntPtr> _globalExport = new Dictionary<string, IntPtr>();
        private readonly Dictionary<string, IntPtr> _globalExportWeak = new Dictionary<string, IntPtr>();

        private readonly byte* _memory;
        private byte* _baseAddress;
        private readonly MachoHeaderNative* _header;
        private readonly MachoLoadCommandCollection _collection;
        private readonly MachoDyldInfoOnlyLoadCommand _infoOnly;
        private readonly MachoSymbolTableLoadCommand _symTab;

        private readonly MachoRebaseCollection _rebase;
        private readonly MachoBindCollection _bind;
        private readonly MachoBindCollection _lazyBind;
        private readonly MachoBindCollection _weakBind;
        private readonly MachoExportCollection _export;

        private readonly MachoSegmentLoadCommand _linkEdit;

        private readonly List<MachoSegmentLoadCommand> _segments;
        private readonly List<MachoDylibLoadCommand> _dylibs;

        private readonly IntPtr[] _dylibHandles;

        private readonly Dictionary<string, IntPtr> _exportMap = new Dictionary<string, IntPtr>();

        public MachoLoadCommandCollection Commands => _collection;

        public MachoRebaseCollection Rebase => _rebase;
        public MachoBindCollection Bindings => _bind;
        public MachoBindCollection LazyBindings => _lazyBind;
        public MachoBindCollection WeakBindings => _weakBind;
        public MachoExportCollection Exports => _export;

        public List<MachoSegmentLoadCommand> Segments => _segments;
        public List<MachoDylibLoadCommand> Dylibs => _dylibs;

        public MachoCpuType CpuType => _header->cputype;

        private MachoHeader(byte* data, bool runtime)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _memory = data;
            _header = (MachoHeaderNative*)data;
            _collection = new MachoLoadCommandCollection(_memory, (ulong)sizeof(MachoHeaderNative), _header->ncmds);
            _infoOnly = _collection.OfType<MachoDyldInfoOnlyLoadCommand>().FirstOrDefault();
            _symTab = _collection.OfType<MachoSymbolTableLoadCommand>().FirstOrDefault();

            if (_infoOnly != null && !runtime)
            {
                _rebase = new MachoRebaseCollection(_memory, _infoOnly.RebaseOffset, _infoOnly.RebaseSize);
                _bind = new MachoBindCollection(_memory, _infoOnly.BindOffset, _infoOnly.BindSize);
                _lazyBind = new MachoBindCollection(_memory, _infoOnly.LazyBindOffset, _infoOnly.LazyBindSize);
                _weakBind = new MachoBindCollection(_memory, _infoOnly.WeakBindOffset, _infoOnly.WeakBindSize);
                _export = new MachoExportCollection(_memory, _infoOnly.ExportOffset, _infoOnly.ExportSize);
            }

            _segments = _collection.OfType<MachoSegmentLoadCommand>().ToList();
            _dylibs = _collection.OfType<MachoDylibLoadCommand>().ToList();

            _linkEdit = _segments.FirstOrDefault(seg => seg.Name == "__LINKEDIT");

            _dylibHandles = new IntPtr[_dylibs.Count];
        }

        public MachoHeader(byte* data) : this(data, false)
        {

        }

        /// <summary>
        /// Inspect an already loaded Macho binary.
        /// </summary>
        /// <param name="data">The file's data</param>
        /// <param name="runtimeAddress">The runtime address of the library</param>
        internal MachoHeader(byte* data, byte* runtimeAddress) : this(data, true)
        {
            _baseAddress = runtimeAddress;
        }

        public Dictionary<string, IntPtr> BuildRuntimeSymbolTable()
        {
            // Some dylibs load at specific addresses.
            ulong firstSegmentVirtual = _segments.FirstOrDefault()?.VirtualAddress ?? 0;

            var result = new Dictionary<string, IntPtr>();

            var symbolOffsetFromLinkedit = _symTab.SymbolTableOffset - _linkEdit.Offset;
            var stringOffsetFromLinkedit = _symTab.StringTableOffset - _linkEdit.Offset;
            var stringTablePtr = _baseAddress + _linkEdit.VirtualAddress - firstSegmentVirtual + stringOffsetFromLinkedit;

            var symbolTable = new MachoSymbolTableArray(_baseAddress, _linkEdit.VirtualAddress - firstSegmentVirtual + symbolOffsetFromLinkedit, _symTab.SymbolCount);
         
            foreach (var sym in symbolTable)
            {
                try
                {
                    var name = Marshal.PtrToStringAnsi((IntPtr)(stringTablePtr + sym.StringTableIndex));
                    
                    // Probably some lazy bound imported symbol.
                    if (sym.Value == 0)
                    {
                        continue;
                    }
                    
                    var value = _baseAddress + sym.Value - firstSegmentVirtual;

                    if (!result.ContainsKey(name))
                    {
                        result.Add(name, (IntPtr)value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Fail: {e}");
                }
            }

            return result;
        }

        public override string ToString()
        {
            return
$@"Magic:         {_header->magic}
CPU type:      {_header->cputype}
CPU subtype:   {_header->cpusubtype.ToString(_header->cputype)}
File type:     {_header->filetype}
Command count: {_header->ncmds}
Command size:  {_header->sizeofcmds}
Flags:         {_header->flags}";
        }
    }
}
