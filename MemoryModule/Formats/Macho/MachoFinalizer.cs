using MemoryModule.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    class MachoFinalizer : IFinalizer
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void FiniDelegate();

        private FiniDelegate _del;

        public IntPtr Address { get; internal set; }

        public void Run()
        {
            _del = Marshal.GetDelegateForFunctionPointer<FiniDelegate>(Address);
            _del();
        }

        internal MachoFinalizer(IntPtr del)
        {
            Address = del;
        }
    }
}
