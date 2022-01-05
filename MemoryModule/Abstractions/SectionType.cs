namespace MemoryModule.Abstractions
{
    public enum SectionType
    {
        /// <summary>
        /// An unknown section.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// A section that usually contains executable code (Mapped as read, execute).
        /// </summary>
        Text = 1,
        /// <summary>
        /// A section that usually contains variables (Mapped as read, write).
        /// </summary>
        Data = 2,
        /// <summary>
        /// A section that usually contains const variables, such as C string literals (Mapped as read).
        /// </summary>
        DataConst = 3,
        /// <summary>
        /// Thread local variable sections.
        /// </summary>
        Tls = 4
    }
}