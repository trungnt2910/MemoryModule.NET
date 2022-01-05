using MemoryModule.Formats.Elf;
using MemoryModule.Formats.Macho;
using MemoryModule.Formats.PE;
using MemoryModule.Tls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Abstractions
{
    public unsafe static class Loader
    {
        /// <summary>
        /// Maps a module's sections into memory.
        /// </summary>
        /// <param name="module">The target module.</param>
        /// <param name="infrastructure">The native functions (infrastructure) provided by the operating system.</param>
        public static void AllocateSections(Module module, INativeFunctions infrastructure = null)
        {
            infrastructure = infrastructure ?? NativeFunctions.Default;

            // TO-DO: Better section-to page strategy. For now, it is assumed that all sections
            // are mapped contiguously.

            var minAddress = ulong.MaxValue;
            var maxAddress = ulong.MinValue;

            foreach (var section in module.Sections)
            {
                minAddress = Math.Min(section.MemoryOffset, minAddress);
                maxAddress = Math.Max(section.MemoryOffset + section.MemorySize, maxAddress);
            }

            // Assuming that we're on a sane machine whose page size is a power of 2.
            var size = AlignValueUp(maxAddress - minAddress, (ulong)Environment.SystemPageSize);

            // If we want to subtract later, we must add first.
            var truePreferredAddress = (module.PreferredAddress == IntPtr.Zero) ? 0 : (ulong)module.PreferredAddress + minAddress;

            // Map pages with write access first.
            // We subtract from minAddress, as the first page's offset might not be zero!
            var baseAddress = (byte*)infrastructure.VirtualAllocate((IntPtr)truePreferredAddress, size, MemoryProtection.Read | MemoryProtection.Write) - minAddress;
            module.MemoryAddress = (IntPtr)baseAddress;
            module.MemorySize = size;

            foreach (var section in module.Sections)
            {
                // Memory is copied. Mind the type casts, it's strict in C#.
                infrastructure.CopyMemory((IntPtr)(baseAddress + section.MemoryOffset), (IntPtr)((ulong)module.FileAddress + section.FileOffset), section.FileSize);
                // No need to zero the extra bytes. For most platforms, VirtualAllocate's native counterpart has 
                // already done that for us.
            }

            module.AfterAllocation();
        }

        /// <summary>
        /// Relocates the module symbols.
        /// </summary>
        /// <param name="module">The target module.</param>
        /// <param name="infrastructure">The native functions (infrastructure) provided by the operating system.</param>
        public static void PerformRebase(Module module, INativeFunctions infrastructure = null)
        {
            infrastructure = infrastructure ?? NativeFunctions.Default;

            // C#'s integral types are well defined... I guess??
            unchecked
            {
                var delta = (ulong)module.MemoryAddress - (ulong)module.PreferredAddress;
                foreach (var rebase in module.Rebases)
                {
                    var addr = (byte**)((byte*)module.MemoryAddress + rebase.AffectedAddress);
                    if (rebase.IgnoreExistingValue)
                    {
                        *addr = (byte *)(delta + rebase.Addend);
                    }
                    else
                    {
                        *addr += delta + rebase.Addend;
                    }
                }
            }
            module.AfterRebase();
        }

        /// <summary>
        /// Binds the module's symbols to external targets.
        /// </summary>
        /// <param name="module">The target module.</param>
        /// <param name="infrastructure">The native functions (infrastructure) provided by the operating system.</param>
        public static void PerformBinding(Module module, INativeFunctions infrastructure = null)
        {
            infrastructure = infrastructure ?? NativeFunctions.Default;

            var libraries = new Dictionary<string, IntPtr>();
            module.ReferencedLibraryHandles = libraries;

            foreach (var reference in module.ReferencedLibraries)
            {
                // Key clash should never occur.
                libraries.Add(reference, infrastructure.LoadLibrary(reference));
            }

            // To-Do: Lazy bindings, or not?
            foreach (var binding in module.Bindings.Concat(module.LazyBindings))
            {
                var addr = (byte**)((byte*)module.MemoryAddress + binding.AffectedAddress);
                
                if (binding.SymbolIndex != null)
                {
                    var index = (ulong)binding.SymbolIndex;
                    // Module Name should be present. It's stupid to bind from an index out of nowhere, no linker
                    // would do that.
                    if (binding.ModuleName == null)
                    {
                        *addr = (byte*)module.Exports[(int)index].Address + binding.Addend;
                    }
                    else
                    {
                        *addr = (byte*)infrastructure.GetSymbolFromLibrary(libraries[binding.ModuleName], (IntPtr)index) + binding.Addend;
                    }
                }
                else
                {
                    // null means searching from self, all deps, and then global scope.
                    if (binding.ModuleName == null)
                    {
                        // Look from self exports.
                        byte* symbol = (byte*)module.PrivateSymbols.FirstOrDefault(sym => sym.Name == binding.SymbolName)?.Address;

                        // Look from deps
                        if (symbol == null)
                        {
                            foreach (var kvp in libraries)
                            {
                                symbol = (byte*)infrastructure.GetSymbolFromLibrary(kvp.Value, binding.SymbolName);
                                if (symbol == null)
                                {
                                    break;
                                }
                            }
                        }

                        // Last desperate call for RTLD_GLOBAL
                        if (symbol == null)
                        {
                            symbol = (byte*)infrastructure.GetSymbolFromLibrary(IntPtr.Zero, binding.SymbolName);
                        }

                        if (symbol == null)
                        {
                            // Should throw something here.
                        }

                        *addr = symbol + binding.Addend;
                    }
                    else if (binding.ModuleName != string.Empty)
                    {
                        *addr = (byte*)infrastructure.GetSymbolFromLibrary(libraries[binding.ModuleName], binding.SymbolName) + binding.Addend;
                    }
                    // Empty: Seaching from current module. This should not happen.
                    else
                    {
                        *addr = (byte*)module.Exports.FirstOrDefault(sym => sym.Name == binding.SymbolName).Address + binding.Addend;
                    }
                }
            }

            foreach (var binding in module.WeakBindings)
            {
                var addr = (byte**)((byte*)module.MemoryAddress + binding.AffectedAddress);
                byte* symbol = null;

                if (binding.SymbolIndex != null)
                {
                    var index = (ulong)binding.SymbolIndex;
                    if (binding.ModuleName == null)
                    {
                        *addr = (byte*)module.Exports[(int)index].Address + binding.Addend;
                    }
                    else
                    {
                        *addr = (byte*)infrastructure.GetSymbolFromLibrary(libraries[binding.ModuleName], (IntPtr)index) + binding.Addend;
                    }
                }
                else
                {
                    // Look in our dependencies.
                    foreach (var kvp in libraries)
                    {
                        symbol = (byte*)infrastructure.GetSymbolFromLibrary(kvp.Value, binding.SymbolName) + binding.Addend;
                        if (symbol == null)
                        {
                            break;
                        }
                    }

                    // Look from global symbols
                    if (symbol == null)
                    {
                        symbol = (byte*)infrastructure.GetSymbolFromLibrary(IntPtr.Zero, binding.SymbolName) + binding.Addend;
                    }

                    // Finally, look at home, if someone else hasn't defined this symbol yet.
                    if (symbol == null)
                    {
                        symbol = (byte*)module.PrivateSymbols?.FirstOrDefault(sym => sym.Name == binding.SymbolName)?.Address;
                    }

                    // Don't crash the app, just ignore. That's the whole point of weak binding.
                    if (symbol != null)
                    {
                        *addr = symbol + binding.Addend;
                    }
                }
            }

            // The module's ID is bound in this phase:
            if (module.HasTls)
            {
                var realAddress = module.TlsImageAddress;
                if (realAddress == IntPtr.Zero)
                {
                    realAddress = (IntPtr)((ulong)module.MemoryAddress + module.TlsImageOffset);
                }
                module.TlsModuleId = TlsHandler.AssignModId(realAddress, module.TlsFileSize, module.TlsMemorySize);

                foreach (var modIdBindAddr in module.TlsModuleIdBindings)
                {
                    var addr = (byte**)((ulong)module.MemoryAddress + modIdBindAddr);
                    *(IntPtr*)addr = (IntPtr)module.TlsModuleId;
                }

                foreach (var tlsGetAddrBindAddr in module.TlsGetAddrBindings)
                {
                    var addr = (byte**)((ulong)module.MemoryAddress + tlsGetAddrBindAddr);
                    *(IntPtr*)addr = Marshal.GetFunctionPointerForDelegate(TlsHandler.TlsGetAddr);
                }
            }
            
            module.AfterBinding();
        }

        /// <summary>
        /// Protects the allocated pages.
        /// </summary>
        /// <param name="module">The target module.</param>
        /// <param name="infrastructure">The native functions (infrastructure) provided by the operating system.</param>
        public static void PerformPageProtection(Module module, INativeFunctions infrastructure = null)
        {
            infrastructure = infrastructure ?? NativeFunctions.Default;

            // Do this only after rebasing, binding, and evil TLS redirection.
            foreach (var section in module.Sections)
            {
                // Although our sections can theoretically be anywhere, sane binaries don't contain such pages.
                infrastructure.VirtualProtect((IntPtr)((ulong)module.MemoryAddress + section.MemoryOffset), section.MemorySize, section.MemoryProtection);
            }
        }

        /// <summary>
        /// Runs the module constructors, if any are available.
        /// </summary>
        /// <param name="module">The target module.</param>
        public static void PerformInitialization(Module module)
        {
            foreach (var init in module.Initializers)
            {
                init.Run();
            }
        }

        /// <summary>
        /// Runs the module destructors.
        /// </summary>
        /// <param name="module">The target module.</param>
        public static void PerformFinalization(Module module)
        {
            foreach (var fini in module.Finalizers)
            {
                fini.Run();
            }
        }

        /// <summary>
        /// Frees the module's referenced libraries.
        /// </summary>
        /// <param name="module">The target module.</param>
        /// <param name="infrastructure">The native functions (infrastructure) provided by the operating system.</param>
        public static void UnloadReferences(Module module, INativeFunctions infrastructure = null)
        {
            infrastructure = infrastructure ?? NativeFunctions.Default;

            foreach (var kvp in module.ReferencedLibraryHandles)
            {
                infrastructure.FreeLibrary(kvp.Value);
            }
        }

        /// <summary>
        /// Deallocate the library's sections.
        /// </summary>
        /// <param name="module">The target module.</param>
        /// <param name="infrastructure">The native functions (infrastructure) provided by the operating system.</param>
        public static void DeallocateSections(Module module, INativeFunctions infrastructure = null)
        {
            infrastructure = infrastructure ?? NativeFunctions.Default;

            infrastructure.VirtualFree(module.MemoryAddress, module.MemorySize);
        }

        /// <summary>
        /// Detects the file type using its magic number and loads the 
        /// module.
        /// </summary>
        /// <param name="ptr">A pointer to the module's memory</param>
        /// <returns>A module object</returns>
        public static Module Load(IntPtr ptr)
        {
            var magic = new List<byte>(_maxMagicLength);

            for (int i = 1; i <= _maxMagicLength; ++i)
            {
                magic.Add(((byte*)ptr)[i - 1]);
                if (_moduleType.TryGetValue(magic.ToArray(), out Type type))
                {
                    return (Module)Activator.CreateInstance(type, ptr);
                }
            }

            throw new NotSupportedException("Image type not supported.");
        }

        /// <summary>
        /// Registers a new loader type.
        /// </summary>
        /// <typeparam name="T">Type of the module format.</typeparam>
        /// <param name="magicNumber">Magic number of the number, represented as a byte array.</param>
        public static void Register<T>(byte[] magicNumber) where T : Module
        {
            _moduleType.Add(magicNumber, typeof(T));
            _maxMagicLength = Math.Max(_maxMagicLength, magicNumber.Length);
        }

        // Some known formats here.
        private static readonly Dictionary<byte[], Type> _moduleType = new Dictionary<byte[], Type>(new ByteArrayEqualityComparer())
        {
            // 0x7F, 'E', 'L', 'F'
            { new byte[]{ 0x7F, 0x45, 0x4C, 0x46 }, typeof(ElfModule) },
            // DOS header: 'M', 'Z'
            { new byte[]{ 0x4D, 0x5A }, typeof(PeModule) },
            // 0xFEEDFACE 32 Bit Macho
            { new byte[]{ 0xCE, 0xFA, 0xED, 0xFE }, typeof(MachoModule) },
            // 0xFEEDFACF 64 Bit Macho
            { new byte[]{ 0xCF, 0xFA, 0xED, 0xFE }, typeof(MachoModule) },
            //MachoFat = 0xcafebabe,
            //MachoFat64 = 0xcafebabf,
            //Fat Macho not supported.
        };

        private static int _maxMagicLength = 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong AlignValueUp(ulong value, ulong alignment)
        {
            return (value + alignment - 1) & ~(alignment - 1);
        }
    }
}
