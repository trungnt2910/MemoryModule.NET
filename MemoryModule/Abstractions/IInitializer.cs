using System;

namespace MemoryModule.Abstractions
{
    public interface IInitializer
    {
        /// <summary>
        /// Runs the initializer.
        /// </summary>
        /// <returns>A boolean that indicates whether the initializer succeeded.</returns>
        bool Run();
        /// <summary>
        /// Runs the initializer with the specified arguments, which may or may not be loaded.
        /// </summary>
        /// <param name="args">Platform specific arguments</param>
        /// <returns>A boolean that indicates whether the initializer succeeded.</returns>
        bool Run(params object[] args);
        /// <summary>
        /// Module-specific arguments required.
        /// This is often an <see cref="IntPtr"/> and an enum for PE binaries, and
        /// <c>int argc, char** argv</c> for ELF modules.
        /// </summary>
        Type[] Arguments { get; }
    }
}