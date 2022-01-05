using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    unsafe class ElfModule : Module
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr TlsGetAddrProc(IntPtr TlvIndex);

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
        public override ulong TlsMemorySize { get; protected set; }
        public override ulong TlsFileSize { get; protected set; }
        public override int TlsModuleId { get; internal set; }
        public override IReadOnlyList<ulong> TlsModuleIdBindings { get; protected set; }
        public override IReadOnlyList<ulong> TlsGetAddrBindings { get; protected set; }
        public override IntPtr TlsImageAddress { get; protected set; }
        public override ulong TlsImageOffset { get; protected set; }

        private ElfHeader _header;

        public ElfModule(IntPtr handle) : this((byte*)handle)
        {
            
        }

        public ElfModule(byte* data)
        {
            FileAddress = (IntPtr)data;
            _header = new ElfHeader(data);

            Architecture = ConvertArchitecture(_header.Architecture);
            
            // LOAD type program headers are equivalent to sections.
            Sections = _header.ProgramHeaders.Where(header => header.Type == ElfProgramHeaderType.Load).Cast<ISection>().ToList();

            // Referenced libraries:
            var dynSection = _header.DynamicSectionHeader;
            var items = dynSection.DynamicSectionItems.ToList();

            var strTable = items.First(item => item.Tag == ElfDynamicSectionItemType.StrTab);

            ReferencedLibraries = items
                .Where(item => item.Tag == ElfDynamicSectionItemType.Needed)
                .Select(item => Marshal.PtrToStringAnsi((IntPtr)_header.GetAddress((ulong)strTable.Pointer + item.Value)))
                .ToList();

            // Exports.
            var symbolTable = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.SymTab);
            var symbolTableSection = _header.SectionHeaders.First(header => header.Offset == symbolTable.Value);

            var symbolArray = new ElfSymbolArray(_header.GetAddress(symbolTableSection.Offset), 0, symbolTableSection.Size / symbolTableSection.EntrySize);
            var stringTable = new ElfStringTable(_header.GetAddress(), strTable.Value);

            foreach (var symbol in symbolArray)
            {
                symbol.ResolveName(stringTable);
            }

            Exports = symbolArray.Where(sym => sym.Binding == ElfSymbolBinding.Global).ToList();

            PrivateSymbols = symbolArray.ToList();

            // Relocations, binds, and weak binds.
            var relaTable = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.Rela);
            var relaSize = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.RelaSz);
            var relaEntrySize = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.RelaEnt);

            var relaArr = relaTable == null ? Enumerable.Empty<ElfRela>() :
                new ElfRelaArray(_header.GetAddress(), relaTable.Value, relaSize.Value / relaEntrySize.Value);

            var jumpRel = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.JmpRel);
            var jumpRelaArr = Enumerable.Empty<ElfRela>();

            if (jumpRel != null)
            {
                var jumpRelSection = _header.SectionHeaders.FirstOrDefault(header => header.Offset == jumpRel.Value);
                jumpRelaArr = new ElfRelaArray(_header.GetAddress(), jumpRelSection.Offset, jumpRelSection.Size / jumpRelSection.EntrySize);
            }

            var _rebases = new List<GenericRebase>();
            var _bindings = new List<GenericBind>();
            var _lazyBindings = new List<GenericBind>();
            var _weakBindings = new List<GenericBind>();

            var tlsAddresses = new List<ulong>();
            var tlsGetAddrAddress = new List<ulong>();

            foreach (var rela in relaArr.Concat(jumpRelaArr))
            {
                if (rela.Type.IsNone())
                {
                    continue;
                }
                else if (rela.Type.IsRelative())
                {
                    _rebases.Add(new GenericRebase()
                    {
                        AffectedAddress = rela.Offset,
                        Addend = rela.Addend,
                        IgnoreExistingValue = true
                    });
                }
                else if (rela.DependsOnSymbol())
                {
                    var sym = symbolArray[rela.Symbol];

                    // Magic symbol provided by glibc for TLS support.
                    if (sym.Name == "__tls_get_addr")
                    {
                        tlsGetAddrAddress.Add(rela.Offset);
                        continue;
                    }

                    // To-Do: JumpSlots are actually lazy binds, right?
                    var bind = new GenericBind()
                    {
                        AffectedAddress = rela.Offset,
                        // ELF sadly doesn't give us this info.
                        ModuleName = null,
                        SymbolName = sym.Name,
                        Addend = rela.Addend
                    };

                    if (sym.Binding != ElfSymbolBinding.Weak)
                    {
                        _bindings.Add(bind);
                    }
                    else
                    {
                        _weakBindings.Add(bind);
                    }
                }
                // These rebases serves TLS.
                // ModID will be handled by the Loader.
                // TLS offset will be baked into the assembly right in this phase.
                // Furthermore, a special symbol, __tls_get_addr, must be trapped by the runtime 
                // in this phase, rather than allowing it to resolve the glibc's default implementation.
                else if (rela.Type.IsModule())
                {
                   //*affected = (void*)module.map.TlsModuleId;
                   tlsAddresses.Add(rela.Offset);
                }
                else if (rela.Type.IsOffset())
                {
                //    var currentSymbol = module.symbolArray[rela.Symbol];
                //    *affected = (void*)currentSymbol.Value;
                    var bind = new GenericBind()
                    {
                        AffectedAddress = rela.Offset,
                        ModuleName = string.Empty,
                        SymbolName = symbolArray[rela.Symbol].Name,
                        Addend = rela.Addend
                    };

                    _bindings.Add(bind);
                }
                else
                {
                    throw new NotImplementedException($"Unimplemented relocation type: {rela.Type}");
                }
            }

            Rebases = _rebases;
            Bindings = _bindings;
            LazyBindings = _lazyBindings;
            WeakBindings = _weakBindings;

            TlsModuleIdBindings = tlsAddresses;
            TlsGetAddrBindings = tlsGetAddrAddress;

            // TLS applies to this module: 
            if (TlsModuleIdBindings.Count != 0)
            {
                var tlsSection = _header.ProgramHeaders.First(header => header.Type == ElfProgramHeaderType.ThreadLocalStorage);
                TlsImageOffset = tlsSection.MemoryOffset;
                TlsFileSize = tlsSection.FileSize;
                TlsMemorySize = tlsSection.MemorySize;
            }

            // That's all we know during construction.
        }

        private static IntPtr TlsGetAddr(IntPtr TlvIndex)
        {
            Console.WriteLine("Boo!");
            return (IntPtr)6969;
        }

        // Here, rebase should be complete, and MemoryAddress should be valid.
        internal override void AfterRebase()
        {
            // Now symbols have their correct addresses.
            foreach (ElfSymbol sym in PrivateSymbols)
            {
                if (sym.Type != ElfSymbolType.TLS && sym.Value != 0)
                {
                    sym.Address = (IntPtr)(sym.Value + (ulong)MemoryAddress);
                }
                else
                {
                    sym.Address = (IntPtr)sym.Value;
                }
            }

            var dynSection = _header.DynamicSectionHeader;
            var items = dynSection.DynamicSectionItems.ToList();

            // Initializers.
            var initItem = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.Init);
            var initArrItem = items.First(item => item.Tag == ElfDynamicSectionItemType.InitArray);
            var initArrSizeItem = items.First(item => item.Tag == ElfDynamicSectionItemType.InitArraySz);
            var initArrCount = initArrSizeItem.Value / (ulong)sizeof(IntPtr);

            var init = new List<ElfInitializer>();
            init.Add(new ElfInitializer((IntPtr)((ulong)MemoryAddress + initItem.Value)));

            var initPtrs = new IntPtr[initArrCount];

            // Must be the real codeBase, not the file.
            Marshal.Copy((IntPtr)((ulong)MemoryAddress + initArrItem.Value), initPtrs, 0, (int)initArrCount);

            foreach (var initPtr in initPtrs)
            {
                // These functions are called AFTER relocation, therefore,
                // they must not be manually "relocated" by adding to the module.codeBase
                init.Add(new ElfInitializer(initPtr));
            }

            Initializers = init;

            // Finalizers
            var finiItem = items.FirstOrDefault(item => item.Tag == ElfDynamicSectionItemType.Fini);
            var finiArrItem = items.First(item => item.Tag == ElfDynamicSectionItemType.FiniArray);
            var finiArrSizeItem = items.First(item => item.Tag == ElfDynamicSectionItemType.FiniArraySz);
            var finiArrCount = finiArrSizeItem.Value / (ulong)sizeof(IntPtr);

            var fini = new List<ElfFinalizer>();
            fini.Add(new ElfFinalizer((IntPtr)((ulong)MemoryAddress + finiItem.Value)));

            var finiPtrs = new IntPtr[finiArrCount];

            // Must be the real codeBase, not the file.
            Marshal.Copy((IntPtr)((ulong)MemoryAddress + finiArrItem.Value), finiPtrs, 0, (int)finiArrCount);

            foreach (var finiPtr in finiPtrs)
            {
                // These functions are called AFTER relocation, therefore,
                // they must not be manually "relocated" by adding to the module.codeBase
                fini.Add(new ElfFinalizer(finiPtr));
            }

            Finalizers = fini;

            //// Resolve __tls_get_addr as the normal bindings won't.
            //var ptrTlsGetAddr = Marshal.GetFunctionPointerForDelegate<TlsGetAddrProc>(TlsGetAddr);
            //foreach (var offset in TlsGetAddrBindings)
            //{
            //    var addr = (byte**)((ulong)MemoryAddress + offset);
            //    *addr = (byte*)ptrTlsGetAddr;
            //    Console.WriteLine($"Address at 0x{(ulong)addr:x} bound to TlsGetAddr");
            //}

            TlsImageAddress = (IntPtr)((ulong)MemoryAddress + TlsImageOffset);
        }

        private static Architecture ConvertArchitecture(ElfArchitecture architecture)
        {
            switch (architecture)
            {
                case ElfArchitecture.ARM:
                    return Architecture.Arm;
                case ElfArchitecture.ARM_64:
                    return Architecture.Arm64;
                case ElfArchitecture.x86_64:
                    return Architecture.X64;
                case ElfArchitecture.x86:
                    return Architecture.X86;
                default:
                    throw new NotSupportedException("Unsupported architecture.");
            }
        }
    }
}
