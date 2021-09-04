using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule
{
    public class NativeAssemblyLoadException : Exception
    {
        public NativeAssemblyLoadException(string message) : base(message)
        {

        }
        public NativeAssemblyLoadException(string message, Exception e) : base(message, e)
        {
        }
    }
}
