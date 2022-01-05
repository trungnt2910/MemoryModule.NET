using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryModule.Tls
{
    static class TlsHandler
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr TlsGetAddrHandler(IntPtr descriptor);
        public static readonly TlsGetAddrHandler TlsGetAddr = TlsGetAddrInternal;

        private static ulong _globalGeneration = 0;
        private static readonly List<TlsGlobalDescriptor> _globalDescriptor = new List<TlsGlobalDescriptor>();

        [ThreadStatic]
        private static List<TlsMemoryDescriptor> _descriptors;

        public static int AssignModId(IntPtr addr, ulong initSize, ulong size)
        {
            lock (_globalDescriptor)
            {
                ++_globalGeneration;
                var modId = _globalDescriptor.Count;
                _globalDescriptor.Add(new TlsGlobalDescriptor()
                {
                    Generation = _globalGeneration,
                    Size = size,
                    InitSize = initSize,
                    Address = addr
                });
                return modId;
            }
        }

        public static void FreeModId(int modId)
        {
            lock (_globalDescriptor)
            {
                ++_globalGeneration;
                _globalDescriptor[modId].Generation = _globalGeneration;
                _globalDescriptor[modId].Size = 0;

                while (_globalDescriptor.Count > 0 && _globalDescriptor[_globalDescriptor.Count - 1].Size == 0)
                {
                    _globalDescriptor.RemoveAt(_globalDescriptor.Count - 1);
                }
            }
        }

        private static IntPtr TlsGetAddrInternal(IntPtr variableDescriptor)
        {
            ulong modId = (ulong)Marshal.ReadIntPtr(variableDescriptor);
            ulong offset = (ulong)Marshal.ReadIntPtr(IntPtr.Add(variableDescriptor, Marshal.SizeOf<IntPtr>()));

            _descriptors = _descriptors ?? new List<TlsMemoryDescriptor>();

            lock (_descriptors)
            lock (_globalDescriptor)
            {
                if (_descriptors.Count < _globalDescriptor.Count)
                {
                    _descriptors.AddRange(Enumerable.Range(0, _globalDescriptor.Count - _descriptors.Count).Select(x => new TlsMemoryDescriptor()));
                }
                var currentDesc = _descriptors[(int)modId];
                var globalDesc = _globalDescriptor[(int)modId];

                if (currentDesc.Generation < globalDesc.Generation)
                {
                    Marshal.FreeHGlobal(currentDesc.Value);
                    currentDesc.Value = Marshal.AllocHGlobal((int)globalDesc.Size);
                    currentDesc.Generation = globalDesc.Generation;
                    unsafe
                    {
                        Unsafe.CopyBlockUnaligned((byte*)currentDesc.Value, (byte*)globalDesc.Address, (uint)globalDesc.InitSize);
                    }
                }

                return (IntPtr)((ulong)currentDesc.Value + offset);
            }
        }
    }
}
