using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule
{
    public class NativeResolveEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the System.ResolveEventArgs class, specifying the
        /// name of the item to resolve and the assembly whose dependency is being resolved.
        /// </summary>
        /// <param name="name">The name of an item to resolve.</param>
        /// <param name="requestingAssembly">The assembly whose dependency is being resolved.</param>
        public NativeResolveEventArgs(string name, NativeAssembly requestingAssembly = null)
        {
            Name = name;
            RequestingAssembly = requestingAssembly;
        }

        /// <summary>
        /// Gets the name of the item to resolve.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the assembly whose dependency is being resolved.
        /// </summary>
        public NativeAssembly RequestingAssembly { get; private set; }

        /// <summary>
        /// Sets whether the returned assembly should be disposed immediatly
        /// The default is false, but should be set to true
        /// if a newly-loaded NativeAssembly has been returned.
        /// </summary>
        public bool ShouldDisposeAssembly { get; set; } = false;
    }
}
