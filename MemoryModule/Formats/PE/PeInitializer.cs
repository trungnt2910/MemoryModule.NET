using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.PE
{
    unsafe class PeInitializer : IInitializer
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate bool DllEntryProc(IntPtr hinstDLL, DllOperation fdwReason, IntPtr lpReserved);

        public Type[] Arguments { get; } = new[] { typeof(IntPtr), typeof(DllOperation), typeof(IntPtr) };

        private IntPtr _handle;
        private IntPtr _del;

        public PeInitializer(IntPtr handle, IntPtr del)
        {
            _handle = handle;
            _del = del;
        }

        public bool Run()
        {
            var _run = Marshal.GetDelegateForFunctionPointer<DllEntryProc>(_del);
            return _run(_handle, DllOperation.ProcessAttach, IntPtr.Zero);
        }

        public bool Run(params object[] args)
        {
            if (args.Length != Arguments.Length)
            {
                return false;
            }

            for (int i = 0; i < args.Length; ++i)
            {
                if (!Arguments[i].IsAssignableFrom(args[i].GetType()))
                {
                    return false;
                }
            }

            var _run = Marshal.GetDelegateForFunctionPointer<DllEntryProc>(_del);
            return _run((IntPtr)args[0], (DllOperation)args[1], (IntPtr)args[2]);
        }
    }
}
