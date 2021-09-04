using System;
using System.Runtime.InteropServices;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct RtldGlobal
    {
        #region Native struct declaration
        [StructLayout(LayoutKind.Sequential)]
        struct link_namespaces
        {
            void* _ns_loaded; //link_map
            uint _ns_nloaded;
            void* _ns_main_searchlist; //_ns_main_searchlist
            uint _ns_global_scope_alloc;
            uint _ns_global_scope_pending_adds;
            void* libc_map; //link_map
            [StructLayout(LayoutKind.Sequential)]
            struct unique_sym_table
            {
                __rtld_lock_recursive_t @lock;
                [StructLayout(LayoutKind.Sequential)]
                struct unique_sym
                {
                    uint hashval;
                    byte* name;
                    void* sym; //Elf32_Sym
                    void *map; //link_map
                }
                unique_sym* entries;
                UIntPtr size;
                UIntPtr n_elements;
                void* free; // void (*free) (void *)
            }
            unique_sym_table _ns_unique_sym_table;

            r_debug _ns_debug;
        }

        // It's sad that we don't have fixed arrays
        // for either structure types or IntPtrs.
        // So, let the manual fun begin
        #region _dl_ns
        link_namespaces _dl_ns_00;
        link_namespaces _dl_ns_01;
        link_namespaces _dl_ns_02;
        link_namespaces _dl_ns_03;
        link_namespaces _dl_ns_04;
        link_namespaces _dl_ns_05;
        link_namespaces _dl_ns_06;
        link_namespaces _dl_ns_07;
        link_namespaces _dl_ns_08;
        link_namespaces _dl_ns_09;
        link_namespaces _dl_ns_10;
        link_namespaces _dl_ns_11;
        link_namespaces _dl_ns_12;
        link_namespaces _dl_ns_13;
        link_namespaces _dl_ns_14;
        link_namespaces _dl_ns_15;
        #endregion
        // Of course I didn't copy paste this shit,
        // I used C# interactive to print them all for me.

        UIntPtr _dl_nns;
        __rtld_lock_recursive_t _dl_load_lock;
        __rtld_lock_recursive_t _dl_load_write_lock;
        ulong _dl_load_adds; //unsigned long long

        void *_dl_initfirst;
        void *_dl_profile_map; //both link_map

        // long is different on Linux 64 bit than on Windows, be careful
        UIntPtr _dl_num_relocations;
        UIntPtr _dl_num_cache_relocations;

        void* _dl_all_dirs; //r_search_path_elem

        //link_map'll never gonna let me go.
        LinkMap _dl_rtld_map;

        #region _dl_rtld_auditstate
        // Fuck this shit, again.
        AuditState _dl_rtld_auditstate_00;
        AuditState _dl_rtld_auditstate_01;
        AuditState _dl_rtld_auditstate_02;
        AuditState _dl_rtld_auditstate_03;
        AuditState _dl_rtld_auditstate_04;
        AuditState _dl_rtld_auditstate_05;
        AuditState _dl_rtld_auditstate_06;
        AuditState _dl_rtld_auditstate_07;
        AuditState _dl_rtld_auditstate_08;
        AuditState _dl_rtld_auditstate_09;
        AuditState _dl_rtld_auditstate_10;
        AuditState _dl_rtld_auditstate_11;
        AuditState _dl_rtld_auditstate_12;
        AuditState _dl_rtld_auditstate_13;
        AuditState _dl_rtld_auditstate_14;
        AuditState _dl_rtld_auditstate_15;
        #endregion

        // These two are quite useful.
        void* _dl_rtld_lock_recursive; //  void (*_dl_rtld_lock_recursive) (void *);
        void* _dl_rtld_unlock_recursive; //  void (*_dl_rtld_unlock_recursive) (void *);

        uint _dl_x86_feature_1;

        // In the original source, they use two bit fields, each 2 bits.
        // After actual testing, it seems that these bit fields
        // has the same size and alignment as an int.
        uint /*dl_x86_feature_control*/ _dl_x86_feature_control;

        //   int (*_dl_make_stack_executable_hook) (void **);
        void* _dl_make_stack_executable_hook;

        uint _dl_stack_flags;
        byte _dl_tls_dtv_gaps; // a c-style boolean

        public UIntPtr _dl_tls_max_dtv_idx;

        public DtvSlotInfoListNative* _dl_tls_dtv_slotinfo_list;

        public UIntPtr _dl_tls_static_nelem;
        public UIntPtr _dl_tls_static_size;
        public UIntPtr _dl_tls_static_used;
        public UIntPtr _dl_tls_static_align;
        public UIntPtr _dl_tls_static_optional;

        public void* _dl_initial_dtv;
        public UIntPtr _dl_tls_generation;

        public void* _dl_init_static_tls;//  void (*_dl_init_static_tls) (struct link_map *);

        // unintersting struct.
        //struct dl_scope_free_list
        //{
        //    size_t count;
        //    void* list[50];
        //} *_dl_scope_free_list;

        void* _dl_scope_free_list;

        list_t _dl_stack_used;
        list_t _dl_stack_user;

        int _dl_stack_cache_lock;
        #endregion

        public bool TlsDtvHasGaps
        {
            get => Convert.ToBoolean(_dl_tls_dtv_gaps);
            set => _dl_tls_dtv_gaps = Convert.ToByte(value);
        }

        public ulong TlsDtvMaxIndex
        {
            get => (ulong)_dl_tls_max_dtv_idx;
            set => _dl_tls_max_dtv_idx = (UIntPtr)value;
        }

        public ulong TlsStaticSize
        {
            get => (ulong)_dl_tls_static_size;
            set => _dl_tls_static_size = (UIntPtr)value;
        }

        public ulong TlsStaticAlign
        {
            get => (ulong)_dl_tls_static_align;
            set => _dl_tls_static_align = (UIntPtr)value;
        }

        public ulong TlsGeneration
        {
            get => (ulong)_dl_tls_generation;
            set => _dl_tls_generation = (UIntPtr)value;
        }

        public DtvSlotInfoList TlsDtvSlotInfoList => new DtvSlotInfoList(_dl_tls_dtv_slotinfo_list);

        [DllImport("dl")]
        private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string name, int mode = 0x01);

        [DllImport("dl")]
        private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string name);

        public static RtldGlobal* GetInstance()
        {
            var libdl = dlopen("libdl.so");
            return (RtldGlobal*)dlsym(libdl, "_rtld_global");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct __rtld_lock_recursive_t
    {
        public pthread_mutex_t mutex;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct list_t
    {
        list_t* next;
        list_t* prev;
    }
}
