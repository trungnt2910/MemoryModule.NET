using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MemoryModule
{
    public class NativeAssembly : IDisposable
    {
        private IntPtr _handle;
        private bool _disposedValue;

        private NativeAssembly(IntPtr handle)
        {
            _handle = handle;
        }

        public static NativeAssembly Load(byte[] data)
        {
            return new NativeAssembly(NativeAssemblyImpl.LoadLibrary(data));
        }

        public void Unload()
        {
            IntPtr deadHandle = IntPtr.Zero;
            deadHandle = Interlocked.Exchange(ref _handle, deadHandle);
            if (deadHandle != IntPtr.Zero)
            {
                NativeAssemblyImpl.FreeLibrary(deadHandle);
            }
        }

        public T GetDelegate<T>(string name) where T : Delegate
        {
            return NativeAssemblyImpl.GetDelegate<T>(_handle, name);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Unload();
                _disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~NativeAssembly()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
