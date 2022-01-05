namespace MemoryModule.Abstractions
{
    public interface IRebase
    {
        /// <summary>
        /// Offset to affected address from the start of the module's memory.
        /// </summary>
        ulong AffectedAddress { get; }
        /// <summary>
        /// An optional addend.
        /// </summary>
        ulong Addend { get; }
        /// <summary>
        /// Ignores the existing value in the current memory slot.
        /// </summary>
        bool IgnoreExistingValue { get; }
    }
}