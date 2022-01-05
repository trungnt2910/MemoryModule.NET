using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    unsafe class MachoModule : Module
    {
        public override IntPtr FileAddress { get; protected set; }
        public override IntPtr MemoryAddress { get; internal set; }
        public override ulong MemorySize { get; internal set; }
        public override IntPtr PreferredAddress { get; protected set; }
        public override Architecture Architecture { get; protected set; }
        public override IReadOnlyList<ISection> Sections { get; protected set; }
        public override IReadOnlyList<IRebase> Rebases { get; protected set; }
        public override IReadOnlyList<IBind> Bindings { get; protected set; }
        public override IReadOnlyList<IBind> LazyBindings { get; protected set; }
        public override IReadOnlyList<IBind> WeakBindings { get; protected set; }
        public override IReadOnlyList<IInitializer> Initializers { get; protected set; }
        public override IReadOnlyList<IFinalizer> Finalizers { get; protected set; }
        public override IReadOnlyList<ISymbol> Exports { get; protected set; }
        public override IReadOnlyList<ISymbol> PrivateSymbols { get; protected set; }
        public override IReadOnlyList<string> ReferencedLibraries { get; protected set; }
        public override IReadOnlyDictionary<string, IntPtr> ReferencedLibraryHandles { get; internal set; }
        public override IReadOnlyList<ulong> TlsModuleIdBindings { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override IReadOnlyList<ulong> TlsGetAddrBindings { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override ulong TlsMemorySize { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override ulong TlsFileSize { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override int TlsModuleId { get => throw new NotImplementedException(); internal set => throw new NotImplementedException(); }
        public override IntPtr TlsImageAddress { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override ulong TlsImageOffset { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        private readonly MachoHeader _header;

        public MachoModule(IntPtr data) : this((byte*)data)
        {

        }

        public MachoModule(byte* data)
        {
            _header = new MachoHeader(data);

            FileAddress = (IntPtr)data;
            Architecture = ConvertArchitecture(_header.CpuType);

            // It's fair to say that the whole managed Module class is inspired by Macho.
            Sections = _header.Segments;

            // MachoRebases are in terms of segment index and segment offset.
            // We need to convert the format to affected virtual address that the loader uses.
            var rebases = new List<GenericRebase>();
            foreach (var rebase in _header.Rebase)
            {
                rebases.Add(new GenericRebase()
                {
                    Addend = 0,
                    AffectedAddress = _header.Segments[(int)rebase.SegmentIndex].MemoryOffset + rebase.SegmentOffset,
                    IgnoreExistingValue = false
                });
            }
            Rebases = rebases;

            List<GenericBind> ConvertBind(MachoBindCollection collection)
            {
                var binds = new List<GenericBind>();
                foreach (var bind in collection)
                {
                    /// To do: This function assumes that all binds are of type
                    /// <see cref="MachoBindType.Pointer"/>
                    /// Some more work may need to be done if some type of
                    /// <see cref="MachoBindType.TextAbsolute32"/> or
                    /// <see cref="MachoBindType.TextPcrel32"/> suddenly crops up.
                    binds.Add(new GenericBind()
                    {
                        AffectedAddress = _header.Segments[(int)bind.SegmentIndex].MemoryOffset + bind.SegmentOffset,
                        Addend = 0,
                        ModuleName = _header.Dylibs[(int)bind.LibraryOrdinal].Name,
                        SymbolName = StripUnderscorePrefix(bind.Name),
                    });
                }
                return binds;
            }

            Bindings = ConvertBind(_header.Bindings);
            LazyBindings = ConvertBind(_header.LazyBindings);
            WeakBindings = ConvertBind(_header.WeakBindings);

            ReferencedLibraries = _header.Dylibs.Skip(1).Select(dylib => dylib.Name).ToList();
        }

        internal override void AfterRebase()
        {
            // Intializers:
            var initList = new List<MachoInitializer>();
            foreach (var seg in _header.Segments)
            {
                foreach (var sect in seg.Sections)
                {
                    if (sect.Type == MachoSectionType.ModInitFuncPointers)
                    {
                        byte* addr = (byte*)MemoryAddress + sect.MemoryAddress;
                        ulong size = sect.MemorySize / (ulong)sizeof(IntPtr);
                        IntPtr* init = (IntPtr*)addr;
                        for (ulong i = 0; i < size; ++i)
                        {
                            initList.Add(new MachoInitializer((IntPtr)((ulong)MemoryAddress + (ulong)init[i])));
                        }
                    }
                }
            }

            Initializers = initList;

            // Finalizers:
            var finiList = new List<MachoFinalizer>();
            foreach (var seg in _header.Segments)
            {
                foreach (var sect in seg.Sections)
                {
                    if (sect.Type == MachoSectionType.ModTermFuncPointers)
                    {
                        byte* addr = (byte*)MemoryAddress + sect.MemoryAddress;
                        ulong size = sect.MemorySize / (ulong)sizeof(IntPtr);
                        IntPtr* fini = (IntPtr*)addr;
                        for (ulong i = 0; i < size; ++i)
                        {
                            finiList.Add(new MachoFinalizer((IntPtr)((ulong)MemoryAddress + (ulong)fini[i])));
                        }
                    }
                }
            }

            Finalizers = finiList;

            // Some functions expect an empty list.
            Exports = new List<GenericSymbol>();

            // PrivateSymbols (an incomplete list, some functions that need local binding will use this list).
            PrivateSymbols = BuildRuntimeSymbolTable();
        }

        internal override void AfterBinding()
        {
            // Exports:
            var exports = new List<GenericSymbol>();
            foreach (var export in _header.Exports)
            {
                if (export.ReexportName != null)
                {
                    var handle = ReferencedLibraryHandles[_header.Dylibs[(int)export.LibraryOrdinal].Name];
                    exports.Add(new GenericSymbol()
                    {
                        Name = StripUnderscorePrefix(export.Name),
                        Value = export.LibraryOrdinal,
                        Address = NativeFunctions.Default.GetSymbolFromLibrary(handle, export.ReexportName)
                    });
                }
                else
                {
                    exports.Add(new GenericSymbol()
                    {
                        Name = StripUnderscorePrefix(export.Name),
                        Value = export.Location,
                        Address = (IntPtr)((byte*)MemoryAddress + export.Location)
                    });
                }
            }

            Exports = exports;

            // PrivateSymbols: Some (re-exports) might be invalid before binding.
            PrivateSymbols = BuildRuntimeSymbolTable();
        }

        private List<GenericSymbol> BuildRuntimeSymbolTable()
        {
            // Some dylibs load at specific addresses.
            ulong firstSegmentVirtual = _header.Segments.FirstOrDefault()?.MemoryOffset ?? 0;

            var result = new List<GenericSymbol>();

            var symTab = _header.Commands.OfType<MachoSymbolTableLoadCommand>().FirstOrDefault();
            var linkEdit = _header.Segments.FirstOrDefault(seg => seg.Name == "__LINKEDIT");
            var baseAddress = (byte*)MemoryAddress;


            var symbolOffsetFromLinkedit = symTab.SymbolTableOffset - linkEdit.FileOffset;
            var stringOffsetFromLinkedit = symTab.StringTableOffset - linkEdit.FileOffset;
            var stringTablePtr = baseAddress + linkEdit.MemoryOffset - firstSegmentVirtual + stringOffsetFromLinkedit;

            var symbolTable = new MachoSymbolTableArray(baseAddress, linkEdit.MemoryOffset - firstSegmentVirtual + symbolOffsetFromLinkedit, symTab.SymbolCount);

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

                    var value = baseAddress + sym.Value - firstSegmentVirtual;

                    result.Add(new GenericSymbol()
                    {
                        Name = StripUnderscorePrefix(name),
                        Value = sym.Value,
                        Address = (IntPtr)value
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Fail: {e}");
                }
            }

            return result;
        }

        private static string StripUnderscorePrefix(string name)
        {
            return (name?.StartsWith("_") ?? false) ? name.Substring(1) : name;
        }

        private static Architecture ConvertArchitecture(MachoCpuType cpuType)
        {
            switch (cpuType)
            {
                case MachoCpuType.I386:
                    return Architecture.X86;
                case MachoCpuType.X86_64:
                    return Architecture.X64;
                case MachoCpuType.ARM:
                case MachoCpuType.ARM64_32:
                    return Architecture.Arm;
                case MachoCpuType.ARM64:
                    return Architecture.Arm64;
                default:
                    throw new NotSupportedException($"Unsupported architecture: {cpuType}");
            }
        }
    }
}
