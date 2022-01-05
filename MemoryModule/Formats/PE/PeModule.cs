using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Iced.Intel;
using MemoryModule.Abstractions;
using MemoryModule.AssemblyHandler;

namespace MemoryModule.Formats.PE
{
    unsafe class PeModule : Module
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

        private readonly DosHeader _dosHeader;
        private readonly PeHeader _ntHeader;

        private readonly PeSectionHeaderArray _sections;
        
        public PeModule(IntPtr memory) : this((byte*)memory)
        {

        }

        public PeModule(byte* memory)
        {
            FileAddress = (IntPtr)memory;

            _dosHeader = new DosHeader(memory);

            ulong ntHeaderOffset = _dosHeader.NewHeaderOffset;
            _ntHeader = new PeHeader(memory, ntHeaderOffset);

            PreferredAddress = (IntPtr)(ulong)_ntHeader.PeOptionalHeader.ImageBase;
            Architecture = ConvertArchitecture(_ntHeader.FileHeader.MachineType);

            // Sections are right after the optional headers.
            ulong sectionOffset = ntHeaderOffset + (ulong)Marshal.OffsetOf<PeHeaderNative>(nameof(PeHeaderNative.OptionalHeader)) + _ntHeader.FileHeader.SizeOfOptionalHeader;
            _sections = new PeSectionHeaderArray(memory, sectionOffset, _ntHeader.FileHeader.NumberOfSections);
            // Dummy section to trick the loader.
            // Apparently sometimes relocation just _don't_ work.
            var dummySectionArray =
                new ISection[]
                {
                    new GenericSection()
                    {
                        FileOffset = 0,
                        FileSize = 0,
                        MemoryOffset = 0,
                        MemorySize = 0,
                    }
                };
            Sections = dummySectionArray.Concat(_sections).ToList();


            var executableSections = _sections.Where(sect => sect.MemoryProtection.HasFlag(MemoryProtection.Execute));

            switch (Architecture)
            {
                case Architecture.X64:
                    PatchX86_64(executableSections, _sections.Last().MemoryOffset + _sections.Last().MemorySize);
                    break;
                case Architecture.X86:
                case Architecture.Arm:
                case Architecture.Arm64:
                    throw new NotSupportedException("TLS patching not supported for this architecture.");
            }

        }

        internal override void AfterAllocation()
        {
            // Rebases (Relocations in PE terminology)
            var baseRelocationDirectory = _ntHeader.PeOptionalHeader.DataDirectory[PeDirectoryEntryType.BaseRelocation];
            Rebases = new PeBaseRelocationBlock((byte *)MemoryAddress, baseRelocationDirectory.VirtualAddress);

            // Bindings (no weak or lazy binds I guess)
            // Because of the nature of PE, we get our ref libraries here too!
            var _refs = new List<string>();
            var _binds = new List<GenericBind>();

            var importDirectory = _ntHeader.PeOptionalHeader.DataDirectory[PeDirectoryEntryType.Import];

            ulong importDirectoryEnd = importDirectory.VirtualAddress + importDirectory.Size;
            // We don't know how many elements so it's best not to 
            for (ulong i = 0; i + (ulong)sizeof(PeImportDescriptorNative) <= importDirectoryEnd; i += (ulong)sizeof(PeImportDescriptorNative))
            {
                var table = new PeImportDescriptor((byte*)MemoryAddress, importDirectory.VirtualAddress + i);
                
                if (table.Name == null)
                {
                    break;
                }

                _refs.Add(table.Name);

                void** thunkRef = (void**)((ulong)MemoryAddress + (table.OriginalFirstThunk == 0 ? table.FirstThunk : table.OriginalFirstThunk));
                void** funcRef = (void**)((ulong)MemoryAddress + table.FirstThunk);

                while (*thunkRef != null)
                {
                    ulong value = (ulong)*thunkRef;

                    var bind = new GenericBind()
                    {
                        Addend = 0,
                        AffectedAddress = (ulong)funcRef - (ulong)MemoryAddress,
                        ModuleName = table.Name,
                    };

                    if (SnapToOrdinal(value, out var newValue))
                    {
                        bind.SymbolIndex = newValue;
                    }
                    else
                    {
                        var hintNameEntry = new PeHintNameTableEntry((byte*)MemoryAddress, value);
                        bind.SymbolName = hintNameEntry.Name;
                    }

                    _binds.Add(bind);

                    ++thunkRef;
                    ++funcRef;
                }
            }

            ReferencedLibraries = _refs;
            Bindings = _binds;
            LazyBindings = new List<GenericBind>();
            WeakBindings = new List<GenericBind>();
        }

        internal override void AfterRebase()
        {
            // Now to the exports.
            var exportDirectory = _ntHeader.PeOptionalHeader.DataDirectory[PeDirectoryEntryType.Export];
            var exportTable = new PeExportDirectory((byte*)MemoryAddress, exportDirectory.VirtualAddress);

            var list = new List<PeExportSymbol>((int)exportTable.NameCount);

            // This array always store 32-bit offsets.
            uint* nameList = (uint*)((ulong)MemoryAddress + exportTable.NameArrayOffset);
            // Same for the funcList, they are not functions addresses, just 32-bit RVAs.
            uint* funcList = (uint*)((ulong)MemoryAddress + exportTable.AddressArrayOffset);
            ushort* ordinalTable = (ushort*)((ulong)MemoryAddress + exportTable.OrdinalArrayOffset);

            for (ulong i = 0; i < exportTable.NameCount; ++i)
            {
                list.Add(new PeExportSymbol()
                {
                    Value = ordinalTable[i],
                    Name = Marshal.PtrToStringAnsi(IntPtr.Add(MemoryAddress, (int)nameList[i])),
                    Address = (IntPtr)((ulong)MemoryAddress + funcList[ordinalTable[i]])
                });
            }

            Exports = list;

            // Finally, init and fini.
            if (_ntHeader.PeOptionalHeader.EntryPointOffset != 0)
            {
                Initializers = new List<PeInitializer>()
                {
                    new PeInitializer(MemoryAddress, IntPtr.Add(MemoryAddress, (int)_ntHeader.PeOptionalHeader.EntryPointOffset))
                };
                Finalizers = new List<PeFinalizer>()
                {
                    new PeFinalizer(MemoryAddress, IntPtr.Add(MemoryAddress, (int)_ntHeader.PeOptionalHeader.EntryPointOffset))
                };
            }
        }

        private static bool SnapToOrdinal(ulong value, out ulong newValue)
        {
            const ulong OrdinalFlag64 = 0x8000000000000000;
            const ulong OrdinalFlag32 = 0x80000000;

            var flag = Environment.Is64BitProcess ? OrdinalFlag64 : OrdinalFlag32;

            if ((value & flag) != 0)
            {
                newValue = value & ~flag;
                return true;
            }
            newValue = value;
            return false;
        }

        private static Architecture ConvertArchitecture(PeMachineType machineType)
        {
            switch (machineType)
            {
                case PeMachineType.I386:
                    return Architecture.X86;
                case PeMachineType.Amd64:
                    return Architecture.X64;
                case PeMachineType.Arm:
                    return Architecture.Arm;
                case PeMachineType.Arm64:
                    return Architecture.Arm64;
                default:
                    throw new NotSupportedException("Unsupported architecture.");
            }
        }

        private void PatchX86_64(IEnumerable<ISection> sections, ulong imageTlsMemoryOffset)
        {
            if (imageTlsMemoryOffset > int.MaxValue)
            {
                throw new NotSupportedException("TLS hooks for image larger than 2GB is not supported.");
            }

            foreach (var sect in sections)
            {
                var sectAddr = (byte*)FileAddress + sect.FileOffset;
                var decoder = Iced.Intel.Decoder.Create(64, new UnsafeNativeMemoryCodeReader(sectAddr, sect.FileSize));
                decoder.IP = (ulong)sectAddr;
                var endRip = (ulong)sectAddr + sect.MemorySize;

                while (decoder.IP < endRip)
                {
                    var instr = decoder.Decode();

                    if (instr.SegmentPrefix != Register.GS)
                    {
                        continue;
                    }

                    if (!instr.HasOpKind(OpKind.Memory))
                    {
                        continue;
                    }

                    if (instr.MemoryDisplacement64 != 11ul * (ulong)sizeof(IntPtr))
                    {
                        continue;
                    }

                    if (instr.OpCount != 2)
                    {
                        continue;
                    }

                    if (instr.Op0Kind != OpKind.Register)
                    {
                        continue;
                    }

                    Console.WriteLine(instr.ToString());
                    Console.WriteLine($"0x{instr.IP:x}");
                    Console.WriteLine(instr.Op0Register);
                    Console.WriteLine(instr.MemoryDisplacement64);
                    Console.WriteLine(instr.SegmentPrefix);

                    var sectOffset = instr.IP - (ulong)sectAddr;
                    var memoryAddress = sect.MemoryOffset + sectOffset + (ulong)instr.Length;

                    var rel32 = imageTlsMemoryOffset - memoryAddress;

                    var asm = new Assembler(64);
                    
                    //var code = Tls.x86_64.HookGenerator.GenerateHookFunction(instr.Op0Register, instr.Op0Register, )
                }
            }
        }
    }
}
