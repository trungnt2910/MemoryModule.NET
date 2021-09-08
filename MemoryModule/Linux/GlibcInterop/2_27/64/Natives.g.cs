// Native structure offsets for glibc, on x86 and x86_64.
// Generated using GlibcOffsetGen. Do not modify.

using System;
using System.Runtime.InteropServices;
namespace GlibcInterop
{
    [StructLayout(LayoutKind.Explicit, Size = 3960)]
    unsafe partial struct rtld_global_2_27_64
    {
        [FieldOffset(0)]
        fixed byte _dl_ns[2304];
        [FieldOffset(2304)]
        fixed byte _dl_nns[8];
        [FieldOffset(2312)]
        fixed byte _dl_load_lock[40];
        [FieldOffset(2352)]
        fixed byte _dl_load_write_lock[40];
        [FieldOffset(2392)]
        fixed byte _dl_load_adds[8];
        [FieldOffset(2408)]
        fixed byte _dl_cpuclock_offset[8];
        [FieldOffset(2424)]
        fixed byte _dl_num_relocations[8];
        [FieldOffset(2432)]
        fixed byte _dl_num_cache_relocations[8];
        [FieldOffset(2448)]
        fixed byte _dl_rtld_map[1136];
        [FieldOffset(3864)]
        fixed byte _dl_stack_flags[4];
        [FieldOffset(3868)]
        fixed byte _dl_tls_dtv_gaps[1];
        [FieldOffset(3872)]
        fixed byte _dl_tls_max_dtv_idx[8];
        [FieldOffset(3888)]
        fixed byte _dl_tls_static_nelem[8];
        [FieldOffset(3896)]
        fixed byte _dl_tls_static_size[8];
        [FieldOffset(3904)]
        fixed byte _dl_tls_static_used[8];
        [FieldOffset(3912)]
        fixed byte _dl_tls_static_align[8];
        [FieldOffset(3928)]
        fixed byte _dl_tls_generation[8];
    }

    [StructLayout(LayoutKind.Explicit, Size = 1136)]
    unsafe partial struct link_map_2_27_64
    {
        [FieldOffset(0)]
        fixed byte l_addr[8];
        [FieldOffset(48)]
        fixed byte l_ns[8];
        [FieldOffset(680)]
        fixed byte l_entry[8];
        [FieldOffset(688)]
        fixed byte l_phnum[2];
        [FieldOffset(690)]
        fixed byte l_ldnum[2];
        [FieldOffset(696)]
        fixed byte l_searchlist[16];
        [FieldOffset(712)]
        fixed byte l_symbolic_searchlist[16];
        [FieldOffset(744)]
        fixed byte l_nversions[4];
        [FieldOffset(748)]
        fixed byte l_nbuckets[4];
        [FieldOffset(752)]
        fixed byte l_gnu_bitmask_idxbits[4];
        [FieldOffset(756)]
        fixed byte l_gnu_shift[4];
        [FieldOffset(784)]
        fixed byte l_direct_opencount[4];
        [FieldOffset(792)]
        fixed byte l_rpath_dirs[16];
        [FieldOffset(832)]
        fixed byte l_map_start[8];
        [FieldOffset(840)]
        fixed byte l_map_end[8];
        [FieldOffset(848)]
        fixed byte l_text_end[8];
        [FieldOffset(888)]
        fixed byte l_scope_max[8];
        [FieldOffset(920)]
        fixed byte l_file_id[16];
        [FieldOffset(936)]
        fixed byte l_runpath_dirs[16];
        [FieldOffset(968)]
        fixed byte l_reldepsmax[4];
        [FieldOffset(972)]
        fixed byte l_used[4];
        [FieldOffset(976)]
        fixed byte l_feature_1[4];
        [FieldOffset(980)]
        fixed byte l_flags_1[4];
        [FieldOffset(984)]
        fixed byte l_flags[4];
        [FieldOffset(988)]
        fixed byte l_idx[4];
        [FieldOffset(992)]
        fixed byte l_mach[24];
        [FieldOffset(1016)]
        fixed byte l_lookup_cache[32];
        [FieldOffset(1056)]
        fixed byte l_tls_initimage_size[8];
        [FieldOffset(1064)]
        fixed byte l_tls_blocksize[8];
        [FieldOffset(1072)]
        fixed byte l_tls_align[8];
        [FieldOffset(1080)]
        fixed byte l_tls_firstbyte_offset[8];
        [FieldOffset(1088)]
        fixed byte l_tls_offset[8];
        [FieldOffset(1096)]
        fixed byte l_tls_modid[8];
        [FieldOffset(1104)]
        fixed byte l_tls_dtor_count[8];
        [FieldOffset(1112)]
        fixed byte l_relro_addr[8];
        [FieldOffset(1120)]
        fixed byte l_relro_size[8];
        [FieldOffset(1128)]
        fixed byte l_serial[8];
        byte* l_audit
        {
            get
            {
                fixed (void* _thisPtr = &this)
                {
                    return (byte*)_thisPtr + 1136;
                }
            }
        }
    }

}
