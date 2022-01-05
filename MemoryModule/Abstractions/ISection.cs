using System;

namespace MemoryModule.Abstractions
{
    public interface ISection
    {
        /// <summary>
        /// Relative offset of this section from the start of the module's memory.
        /// </summary>
        ulong MemoryOffset { get; }
        /// <summary>
        /// Size of the section when loaded in memory.
        /// </summary>
        ulong MemorySize { get; }
        /// <summary>
        /// Offset of this section from the start of the file.
        /// </summary>
        ulong FileOffset { get; }
        /// <summary>
        /// Size of section, on the original file. May be smaller than <see cref="MemorySize"/>.
        /// </summary>
        ulong FileSize { get; }
        /// <summary>
        /// Protection of this section (Read, Write, Execute).
        /// </summary>
        MemoryProtection MemoryProtection { get; }
        /// <summary>
        /// Name of this section (if available).
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Type of this section.
        /// </summary>
        SectionType Type { get; }
    }
}