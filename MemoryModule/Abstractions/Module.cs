using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MemoryModule.Abstractions
{
    public abstract class Module
    {
        /// <summary>
        /// The module's file address in memory.
        /// </summary>
        public abstract IntPtr FileAddress { get; protected set; }
        /// <summary>
        /// The module's address when loaded in memory.
        /// </summary>
        public abstract IntPtr MemoryAddress { get; internal set; }
        /// <summary>
        /// The module's size when loaded in memory.
        /// </summary>
        public abstract ulong MemorySize { get; internal set; }
        /// <summary>
        /// The preferred address of a module. If successfully loaded here, 
        /// rebases can be ignored.
        /// </summary>
        public abstract IntPtr PreferredAddress { get; protected set; }
        /// <summary>
        /// The module's architecture.
        /// </summary>
        public abstract Architecture Architecture { get; protected set; }
        /// <summary>
        /// Parts of the module that must be mapped and copied into memory.
        /// </summary>
        public abstract IReadOnlyList<ISection> Sections { get; protected set; }
        /// <summary>
        /// Locations that must be added to the base address of the module when loaded in memory.
        /// </summary>
        public abstract IReadOnlyList<IRebase> Rebases { get; protected set; }
        /// <summary>
        /// Symbols that must be resolved in the module before initialization.
        /// </summary>
        public abstract IReadOnlyList<IBind> Bindings { get; protected set; }
        /// <summary>
        /// Symbols that can be lazily bound, if the implementation supports.
        /// </summary>
        public abstract IReadOnlyList<IBind> LazyBindings { get; protected set; }
        /// <summary>
        /// Symbols that may or may not be bound. Useful for C++ template statics.
        /// </summary>
        public abstract IReadOnlyList<IBind> WeakBindings { get; protected set; }
        /// <summary>
        /// Addresses that must be set to the current module ID.
        /// </summary>
        public abstract IReadOnlyList<ulong> TlsModuleIdBindings { get; protected set; }
        /// <summary>
        /// Addresses that must be set to the managed TlsGetAddr implementation.
        /// </summary>
        public abstract IReadOnlyList<ulong> TlsGetAddrBindings { get; protected set; }
        /// <summary>
        /// Size of the module's Tls region.
        /// </summary>
        public abstract ulong TlsMemorySize { get; protected set; }
        /// <summary>
        /// Size of the module's Tls image.
        /// </summary>
        public abstract ulong TlsFileSize { get; protected set; }
        /// <summary>
        /// The module ID assigned by the Loader.
        /// </summary>
        public abstract int TlsModuleId { get; internal set; }
        /// <summary>
        /// The module's TLS image address.
        /// </summary>
        public abstract IntPtr TlsImageAddress { get; protected set; }
        /// <summary>
        /// The module's TLS image region, relative to its memory address.
        /// </summary>
        public abstract ulong TlsImageOffset { get; protected set; }
        /// <summary>
        /// Checks whether this module has TLS.
        /// </summary>
        public virtual bool HasTls => TlsMemorySize != 0;
        /// <summary>
        /// Module initializers. Run when module finishes loading.
        /// </summary>
        public abstract IReadOnlyList<IInitializer> Initializers { get; protected set; }
        /// <summary>
        /// Module finalizers. Run before module is unloaded.
        /// </summary>
        public abstract IReadOnlyList<IFinalizer> Finalizers { get; protected set; }
        /// <summary>
        /// List of exported symbols. This symbols are often accessible through <c>dlsym</c>.
        /// </summary>
        public abstract IReadOnlyList<ISymbol> Exports { get; protected set; }
        /// <summarygood 
        /// List of private symbols, if available (i.e. Macho), <c>null</c> on other platforms.
        /// </summary>
        public abstract IReadOnlyList<ISymbol> PrivateSymbols { get; protected set; }
        /// <summary>
        /// List of referenced libraries.
        /// </summary>
        public abstract IReadOnlyList<string> ReferencedLibraries { get; protected set; }
        /// <summary>
        /// Table of opened library handles.
        /// </summary>
        public abstract IReadOnlyDictionary<string, IntPtr> ReferencedLibraryHandles { get; internal set; }
        /// <summary>
        /// Updates values after allocation.
        /// </summary>
        internal virtual void AfterAllocation()
        {

        }
        /// <summary>
        /// Updates values after rebasing.
        /// </summary>
        internal virtual void AfterRebase()
        {

        }
        /// <summary>
        /// Updates values after binding.
        /// </summary>
        internal virtual void AfterBinding()
        {

        }
    }
}
