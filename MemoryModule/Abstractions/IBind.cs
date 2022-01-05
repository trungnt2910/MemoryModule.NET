namespace MemoryModule.Abstractions
{
    public interface IBind
    {
        /// <summary>
        /// Offset to affected address from the start of the module's memory.
        /// </summary>
        ulong AffectedAddress { get; }
        /// <summary>
        /// Module to look for the symbol. Empty string means the current module, null means to search everywhere.
        /// </summary>
        string ModuleName { get; }
        /// <summary>
        /// Symbol to look for.
        /// </summary>
        string SymbolName { get; }
        /// <summary>
        /// Symbol index in the target library. Optional.
        /// </summary>
        ulong? SymbolIndex { get; }
        /// <summary>
        /// An optional addend.
        /// </summary>
        ulong Addend { get; }
    }
}