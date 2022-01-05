using System;

namespace MemoryModule.Abstractions
{
    /// <summary>
    /// A symbol, as defined in the library. It may point to a variable, a function, or nothing at all.
    /// </summary>
    public interface ISymbol
    {
        /// <summary>
        /// Name of the symbol
        /// </summary>
        string Name { get; }
        /// <summary>
        /// "Value" of the symbol, as stated in the library.
        /// </summary>
        ulong Value { get; }
        /// <summary>
        /// Real address of the symbol, in memory. <see cref="IntPtr.Zero"/> if not applicable (such as ELF TLS symbols).
        /// </summary>
        IntPtr Address { get; }
    }
}