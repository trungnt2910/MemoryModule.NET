using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GlibcInterop
{
    public unsafe class RtldGlobal
    {
        // The following fields are set using reflection
#pragma warning disable CS0649
        private static readonly ulong _dl_tls_dtv_gaps_offset;
        private static readonly ulong _dl_tls_max_dtv_idx_offset;
        private static readonly ulong _dl_tls_static_size_offset;
        private static readonly ulong _dl_tls_static_align_offset;
        private static readonly ulong _dl_tls_generation_offset;
        private static readonly ulong _dl_tls_dtv_slotinfo_list_offset;
        private static readonly ulong _dl_tls_static_nelem_offset;
#pragma warning restore CS0649

        private byte* _memory;

        public static readonly RtldGlobal Instance = GetInstance();

        public ulong TlsStaticElementCount
        {
            get => (ulong)*(UIntPtr*)(_memory + _dl_tls_static_nelem_offset);
            set => *(UIntPtr*)(_memory + _dl_tls_static_nelem_offset) = (UIntPtr)value;
        }

        public bool TlsDtvHasGaps
        {
            get => Convert.ToBoolean(*(_memory + _dl_tls_dtv_gaps_offset));
            set => *(_memory + _dl_tls_dtv_gaps_offset) = Convert.ToByte(value);
        }

        public ulong TlsDtvMaxIndex
        {
            get => (ulong)*(UIntPtr*)(_memory + _dl_tls_max_dtv_idx_offset);
            set => *(UIntPtr*)(_memory + _dl_tls_max_dtv_idx_offset) = (UIntPtr)value;
        }

        public ulong TlsStaticSize
        {
            get => (ulong)*(UIntPtr*)(_memory + _dl_tls_static_size_offset);
            set => *(UIntPtr*)(_memory + _dl_tls_static_size_offset) = (UIntPtr)value;
        }

        public ulong TlsStaticAlign
        {
            get => (ulong)*(UIntPtr*)(_memory + _dl_tls_static_align_offset);
            set => *(UIntPtr*)(_memory + _dl_tls_static_align_offset) = (UIntPtr)value;
        }

        public ulong TlsGeneration
        {
            get => (ulong)*(UIntPtr*)(_memory + _dl_tls_generation_offset);
            set => *(UIntPtr*)(_memory + _dl_tls_generation_offset) = (UIntPtr)value;
        }

        public DtvSlotInfoList TlsDtvSlotInfoList => new DtvSlotInfoList((DtvSlotInfoListNative*)(_memory + _dl_tls_dtv_slotinfo_list_offset));

        static RtldGlobal()
        {
            var version = GlibcEnvironment.Version;
            var bitSize = Environment.Is64BitProcess ? "64" : "32";

            var nativeType = Assembly.GetExecutingAssembly()
                .GetType($"GlibcInterop.rtld_global_{version.ToString().Replace('.', '_')}_{bitSize}");

            foreach (var field in nativeType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var offsetField = typeof(RtldGlobal)
                    .GetField($"{field.Name}_offset", BindingFlags.Static | BindingFlags.NonPublic);
                // This field is needed.
                if (offsetField != null)
                {
                    offsetField.SetValue(null, (ulong)Marshal.OffsetOf(nativeType, field.Name));
                }
            }
        }

        private RtldGlobal(byte* memory)
        {
            _memory = memory;
        }

        private static RtldGlobal GetInstance()
        {
            var libdl = dlopen("libdl.so");
            var ptr = dlsym(libdl, "_rtld_global");

            return new RtldGlobal((byte*)ptr);
        }

        [DllImport("dl")]
        private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string name, int mode = 0x01);

        [DllImport("dl")]
        private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string name);
    }

    //[StructLayout(LayoutKind.Sequential)]
    //struct __rtld_lock_recursive_t
    //{
    //    public pthread_mutex_t mutex;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //unsafe struct list_t
    //{
    //    list_t* next;
    //    list_t* prev;
    //}
}
