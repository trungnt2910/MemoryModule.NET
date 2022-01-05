using MemoryModule.Abstractions;
using System;
using System.Runtime.InteropServices;

namespace MemoryModule.Formats.PE
{
    class PeFinalizer : IFinalizer
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate bool DllEntryProc(IntPtr hinstDLL, DllOperation fdwReason, IntPtr lpReserved);

        private IntPtr _handle;
        private IntPtr _del;

        public PeFinalizer(IntPtr handle, IntPtr del)
        {
            _handle = handle;
            _del = del;
        }

        public void Run()
        {
            var _run = Marshal.GetDelegateForFunctionPointer<DllEntryProc>(_del);
            _run(_handle, DllOperation.ProcessDetach, IntPtr.Zero);
        }
    }
}