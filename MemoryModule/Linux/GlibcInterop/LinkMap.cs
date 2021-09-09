using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GlibcInterop
{
    public unsafe class LinkMap : IDisposable
    {
        // The following fields are set using reflection
#pragma warning disable CS0649
        private static readonly ulong l_tls_initimage_offset;
        private static readonly ulong l_tls_initimage_size_offset;
        private static readonly ulong l_tls_blocksize_offset;
        private static readonly ulong l_tls_align_offset;
        private static readonly ulong l_tls_modid_offset;
        private static readonly ulong l_name_offset;
#pragma warning restore CS0649

        private static readonly Type _nativeType;

        private byte* _memory;
        private bool _disposedValue;
        private bool _initFromNative;

        static LinkMap()
        {
            var version = GlibcEnvironment.Version;
            var bitSize = Environment.Is64BitProcess ? "64" : "32";

            _nativeType = Assembly.GetExecutingAssembly()
                .GetType($"GlibcInterop.link_map_{version.ToString().Replace('.', '_')}_{bitSize}");

            foreach (var field in _nativeType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var offsetField = typeof(LinkMap)
                    .GetField($"{field.Name}_offset", BindingFlags.Static | BindingFlags.NonPublic);
                // This field is needed.
                if (offsetField != null)
                {
                    offsetField.SetValue(null, (ulong)Marshal.OffsetOf(_nativeType, field.Name));
                }
            }
        }

        public LinkMap()
        {
            _initFromNative = false;
            _memory = (byte*)Marshal.AllocHGlobal(Marshal.SizeOf(_nativeType));
        }

        public LinkMap(byte* memory)
        {
            _memory = memory;
            _initFromNative = true;
        }

        /// <summary>
        /// The initial contents of TLS memory.
        /// </summary>
        public void* TlsInitialImage
        {
            get => *(void **)(_memory + l_tls_initimage_offset);
            set => *(void**)(_memory + l_tls_initimage_offset) = value;
        }

        /// <summary>
        /// Size of the TLS image stored in the file
        /// </summary>
        public ulong TlsInitImageSize
        {
            get => (ulong)*(UIntPtr*)(_memory + l_tls_initimage_size_offset);
            set => *(UIntPtr*)(_memory + l_tls_initimage_size_offset) = (UIntPtr)value;
        }

        /// <summary>
        /// Size of the TLS block in memory.
        /// </summary>
        public ulong TlsBlockSize
        {
            get => (ulong)*(UIntPtr*)(_memory + l_tls_blocksize_offset);
            set => *(UIntPtr*)(_memory + l_tls_blocksize_offset) = (UIntPtr)value;
        }

        /// <summary>
        /// Alignment requirements for the TLS block
        /// </summary>
        public ulong TlsBlockAlignment
        {
            get => (ulong)*(UIntPtr*)(_memory + l_tls_align_offset);
            set => *(UIntPtr*)(_memory + l_tls_align_offset) = (UIntPtr)value;
        }

        /// <summary>
        /// Id of the module in TLS DTV array.
        /// </summary>
        public ulong TlsModuleId
        {
            get => (ulong)*(UIntPtr*)(_memory + l_tls_modid_offset);
            set => *(UIntPtr*)(_memory + l_tls_modid_offset) = (UIntPtr)value;
        }

        /// <summary>
        /// Name of the library, with its full path.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi(*(IntPtr*)(_memory + l_name_offset));

        public void* GetNativePointer() => _memory;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (!_initFromNative)
                {
                    Marshal.FreeHGlobal((IntPtr)_memory);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
         ~LinkMap()
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
