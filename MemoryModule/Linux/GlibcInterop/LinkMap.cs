using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct LinkMap
    {
        UIntPtr l_addr;
        byte* l_name;
        void* l_ld; //Elf32/64_Dyn
        LinkMap* l_next;
        LinkMap* l_prev;
        LinkMap* l_real;
        UIntPtr l_ns;
        void* l_libname; //libname_list

        // We don't know where these hellish 
        // magic numbers come from, but we do know that
        // they're the same on 32 and 64 bit.
        //    Elf32_Dyn* l_info[35 + 0 + 16
        //+ 3 + 12 + 11];

        #region Don't read this it's a nightmare of void*'s
        void* l_info_00; //Elf(32/64)_Dyn*
        void* l_info_01; //Elf(32/64)_Dyn*
        void* l_info_02; //Elf(32/64)_Dyn*
        void* l_info_03; //Elf(32/64)_Dyn*
        void* l_info_04; //Elf(32/64)_Dyn*
        void* l_info_05; //Elf(32/64)_Dyn*
        void* l_info_06; //Elf(32/64)_Dyn*
        void* l_info_07; //Elf(32/64)_Dyn*
        void* l_info_08; //Elf(32/64)_Dyn*
        void* l_info_09; //Elf(32/64)_Dyn*
        void* l_info_10; //Elf(32/64)_Dyn*
        void* l_info_11; //Elf(32/64)_Dyn*
        void* l_info_12; //Elf(32/64)_Dyn*
        void* l_info_13; //Elf(32/64)_Dyn*
        void* l_info_14; //Elf(32/64)_Dyn*
        void* l_info_15; //Elf(32/64)_Dyn*
        void* l_info_16; //Elf(32/64)_Dyn*
        void* l_info_17; //Elf(32/64)_Dyn*
        void* l_info_18; //Elf(32/64)_Dyn*
        void* l_info_19; //Elf(32/64)_Dyn*
        void* l_info_20; //Elf(32/64)_Dyn*
        void* l_info_21; //Elf(32/64)_Dyn*
        void* l_info_22; //Elf(32/64)_Dyn*
        void* l_info_23; //Elf(32/64)_Dyn*
        void* l_info_24; //Elf(32/64)_Dyn*
        void* l_info_25; //Elf(32/64)_Dyn*
        void* l_info_26; //Elf(32/64)_Dyn*
        void* l_info_27; //Elf(32/64)_Dyn*
        void* l_info_28; //Elf(32/64)_Dyn*
        void* l_info_29; //Elf(32/64)_Dyn*
        void* l_info_30; //Elf(32/64)_Dyn*
        void* l_info_31; //Elf(32/64)_Dyn*
        void* l_info_32; //Elf(32/64)_Dyn*
        void* l_info_33; //Elf(32/64)_Dyn*
        void* l_info_34; //Elf(32/64)_Dyn*
        void* l_info_35; //Elf(32/64)_Dyn*
        void* l_info_36; //Elf(32/64)_Dyn*
        void* l_info_37; //Elf(32/64)_Dyn*
        void* l_info_38; //Elf(32/64)_Dyn*
        void* l_info_39; //Elf(32/64)_Dyn*
        void* l_info_40; //Elf(32/64)_Dyn*
        void* l_info_41; //Elf(32/64)_Dyn*
        void* l_info_42; //Elf(32/64)_Dyn*
        void* l_info_43; //Elf(32/64)_Dyn*
        void* l_info_44; //Elf(32/64)_Dyn*
        void* l_info_45; //Elf(32/64)_Dyn*
        void* l_info_46; //Elf(32/64)_Dyn*
        void* l_info_47; //Elf(32/64)_Dyn*
        void* l_info_48; //Elf(32/64)_Dyn*
        void* l_info_49; //Elf(32/64)_Dyn*
        void* l_info_50; //Elf(32/64)_Dyn*
        void* l_info_51; //Elf(32/64)_Dyn*
        void* l_info_52; //Elf(32/64)_Dyn*
        void* l_info_53; //Elf(32/64)_Dyn*
        void* l_info_54; //Elf(32/64)_Dyn*
        void* l_info_55; //Elf(32/64)_Dyn*
        void* l_info_56; //Elf(32/64)_Dyn*
        void* l_info_57; //Elf(32/64)_Dyn*
        void* l_info_58; //Elf(32/64)_Dyn*
        void* l_info_59; //Elf(32/64)_Dyn*
        void* l_info_60; //Elf(32/64)_Dyn*
        void* l_info_61; //Elf(32/64)_Dyn*
        void* l_info_62; //Elf(32/64)_Dyn*
        void* l_info_63; //Elf(32/64)_Dyn*
        void* l_info_64; //Elf(32/64)_Dyn*
        void* l_info_65; //Elf(32/64)_Dyn*
        void* l_info_66; //Elf(32/64)_Dyn*
        void* l_info_67; //Elf(32/64)_Dyn*
        void* l_info_68; //Elf(32/64)_Dyn*
        void* l_info_69; //Elf(32/64)_Dyn*
        void* l_info_70; //Elf(32/64)_Dyn*
        void* l_info_71; //Elf(32/64)_Dyn*
        void* l_info_72; //Elf(32/64)_Dyn*
        void* l_info_73; //Elf(32/64)_Dyn*
        void* l_info_74; //Elf(32/64)_Dyn*
        void* l_info_75; //Elf(32/64)_Dyn*
        void* l_info_76; //Elf(32/64)_Dyn*
        #endregion

        void* l_phdr; //Elf(32/64)_Phdr

        UIntPtr l_entry;
        short l_phnum;
        short l_ldnum;

        ScopeElement l_searchlist;
        ScopeElement l_symbolic_searchlist;
        LinkMap* l_loader;
        void* l_versions; //r_found_version
        uint l_nversions;

        uint l_nbuckets;
        uint l_gnu_bitmask_idxbits;
        uint l_gnu_shift;
        UIntPtr* l_gnu_bitmask;

        [StructLayout(LayoutKind.Explicit)]
        struct WordPtrAndSymndxPtrUnion
        {
            [FieldOffset(0)]
            uint* a;
            [FieldOffset(0)]
            uint* b;
        }

        WordPtrAndSymndxPtrUnion l_gnu_buckets_or_chain;
        WordPtrAndSymndxPtrUnion l_gnu_chain_zero_or_buckets;

        uint l_direct_opencount;

        uint l_stupid_17_bit_flags;

        byte l_nodelete_active;
        byte l_nodelete_pending;

        // On Linux, this field seems to show up as a short.
        ushort l_property;

        uint l_x86_feature_1_and;
        uint l_x86_isa_1_needed;

        r_search_path_struct l_rpath_dirs;

        //Another uinteresting struct
        //  struct reloc_result
        //  {
        //      Elf32_Addr addr;
        //      struct link_map *bound;
        //unsigned int boundndx;
        //      uint32_t enterexit;
        //      unsigned int flags;



        //      unsigned int init;
        //  } *l_reloc_result;
        void* l_reloc_result;

        void* l_versyms; //Elf32_Versym

        byte* l_origin;

        UIntPtr l_map_start;
        UIntPtr l_map_endl;
        UIntPtr l_text_end;

        // Here we go again
        #region struct r_scope_elem *l_scope_mem[4];
        void* l_scope_mem_00; //r_scope_elem *
        void* l_scope_mem_01; //r_scope_elem *
        void* l_scope_mem_02; //r_scope_elem *
        void* l_scope_mem_03; //r_scope_elem *
        #endregion

        UIntPtr l_scope_max;

        void** l_scope; //r_scope_elem **

        // And more
        #region struct r_scope_elem *l_local_scope[2];
        void* l_local_scope_00; //r_scope_elem *
        void* l_local_scope_01; //r_scope_elem *
        #endregion

        r_file_id l_file_id;

        r_search_path_struct l_runpath_dirs;

        LinkMap** l_initfini;

        // Another unintersting struct
        //  struct link_map_reldeps
        //  {
        //      unsigned int act;
        //      struct link_map *list[];
        //} *l_reldeps;
        void* l_reldeps;

        uint l_reldepsmax;
        uint l_used;

        uint l_feature_1;
        uint l_flags_1;
        uint l_flags;

        int l_idx;

        link_map_machine l_mach;

        [StructLayout(LayoutKind.Sequential)]
        struct l_lookup_cache_t
        {
            void* sym; //Elf(32/64)_Sym
            int type_class;
            LinkMap* value;
            void* ret; //Elf(32/64)_Sym
        }

        l_lookup_cache_t l_lookup_cache;

        void* l_tls_initimage;
        UIntPtr l_tls_initimage_size;
        UIntPtr l_tls_blocksize;
        UIntPtr l_tls_align;
        UIntPtr l_tls_firstbyte_offset;

        UIntPtr l_tls_offset;
        UIntPtr l_tls_modid;
        UIntPtr l_tls_dtor_count;

        UIntPtr l_relro_addr;
        UIntPtr l_relro_size;

        UIntPtr l_serial;

        /// <summary>
        /// The initial contents of TLS memory.
        /// </summary>
        public void* TlsInitialImage
        {
            get => l_tls_initimage;
            set => l_tls_initimage = value;
        }

        /// <summary>
        /// Size of the TLS image stored in the file
        /// </summary>
        public ulong TlsInitImageSize
        {
            get => (ulong)l_tls_initimage_size;
            set => l_tls_initimage_size = (UIntPtr)value;
        }

        /// <summary>
        /// Size of the TLS block in memory.
        /// </summary>
        public ulong TlsBlockSize
        {
            get => (ulong)l_tls_blocksize;
            set => l_tls_blocksize = (UIntPtr)value;
        }

        /// <summary>
        /// Alignment requirements for the TLS block
        /// </summary>
        public ulong TlsBlockAlignment
        {
            get => (ulong)l_tls_align;
            set => l_tls_align = (UIntPtr)value;
        }

        /// <summary>
        /// Id of the module in TLS DTV array.
        /// </summary>
        public ulong TlsModuleId
        {
            get => (ulong)l_tls_modid;
            set => l_tls_modid = (UIntPtr)value;
        }

        /// <summary>
        /// Name of the library, with its full path.
        /// </summary>
        public string Name => Marshal.PtrToStringAnsi((IntPtr)l_name);
    }
}
