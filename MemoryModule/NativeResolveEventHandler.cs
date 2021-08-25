using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule
{
    /// <summary>
    /// Represents a method that handles the MemoryModule.NativeAssembly.AssemblyResolve
    /// event of an MemoryModule.NativeAssembly.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event data.</param>
    /// <returns>The resolved NativeAssembly</returns>
    public delegate NativeAssembly NativeResolveEventHandler(object sender, NativeResolveEventArgs args);
}
