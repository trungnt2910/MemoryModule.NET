// Native structure offsets for glibc, on x86 and x86_64.
// Generated using GlibcOffsetGen. Do not modify.

using System;
using System.Runtime.InteropServices;
namespace GlibcInterop
{
    [StructLayout(LayoutKind.Explicit, Size = 3992)]
    unsafe partial struct rtld_global_2_31_64
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
        [FieldOffset(2416)]
        fixed byte _dl_num_relocations[8];
        [FieldOffset(2424)]
        fixed byte _dl_num_cache_relocations[8];
        [FieldOffset(2440)]
        fixed byte _dl_rtld_map[1152];
        [FieldOffset(3592)]
        fixed byte _dl_rtld_auditstate[256];
        [FieldOffset(3864)]
        fixed byte _dl_x86_feature_1[8];
        [FieldOffset(3872)]
        fixed byte _dl_x86_legacy_bitmap[16];
        [FieldOffset(3896)]
        fixed byte _dl_stack_flags[4];
        [FieldOffset(3900)]
        fixed byte _dl_tls_dtv_gaps[1];
        [FieldOffset(3904)]
        fixed byte _dl_tls_max_dtv_idx[8];
        [FieldOffset(3920)]
        fixed byte _dl_tls_static_nelem[8];
        [FieldOffset(3928)]
        fixed byte _dl_tls_static_size[8];
        [FieldOffset(3936)]
        fixed byte _dl_tls_static_used[8];
        [FieldOffset(3944)]
        fixed byte _dl_tls_static_align[8];
        [FieldOffset(3960)]
        fixed byte _dl_tls_generation[8];
    }

    [StructLayout(LayoutKind.Explicit, Size = 1152)]
    unsafe partial struct link_map_2_31_64
    {
        [FieldOffset(0)]
        fixed byte l_addr[8];
        [FieldOffset(48)]
        fixed byte l_ns[8];
        [FieldOffset(688)]
        fixed byte l_entry[8];
        [FieldOffset(696)]
        fixed byte l_phnum[2];
        [FieldOffset(698)]
        fixed byte l_ldnum[2];
        [FieldOffset(704)]
        fixed byte l_searchlist[16];
        [FieldOffset(720)]
        fixed byte l_symbolic_searchlist[16];
        [FieldOffset(752)]
        fixed byte l_nversions[4];
        [FieldOffset(756)]
        fixed byte l_nbuckets[4];
        [FieldOffset(760)]
        fixed byte l_gnu_bitmask_idxbits[4];
        [FieldOffset(764)]
        fixed byte l_gnu_shift[4];
        [FieldOffset(792)]
        fixed byte l_direct_opencount[4];
        [FieldOffset(799)]
        fixed byte l_nodelete_active[1];
        [FieldOffset(800)]
        fixed byte l_nodelete_pending[1];
        [FieldOffset(808)]
        fixed byte l_rpath_dirs[16];
        [FieldOffset(848)]
        fixed byte l_map_start[8];
        [FieldOffset(856)]
        fixed byte l_map_end[8];
        [FieldOffset(864)]
        fixed byte l_text_end[8];
        [FieldOffset(904)]
        fixed byte l_scope_max[8];
        [FieldOffset(936)]
        fixed byte l_file_id[16];
        [FieldOffset(952)]
        fixed byte l_runpath_dirs[16];
        [FieldOffset(984)]
        fixed byte l_reldepsmax[4];
        [FieldOffset(988)]
        fixed byte l_used[4];
        [FieldOffset(992)]
        fixed byte l_feature_1[4];
        [FieldOffset(996)]
        fixed byte l_flags_1[4];
        [FieldOffset(1000)]
        fixed byte l_flags[4];
        [FieldOffset(1004)]
        fixed byte l_idx[4];
        [FieldOffset(1008)]
        fixed byte l_mach[24];
        [FieldOffset(1032)]
        fixed byte l_lookup_cache[32];
        [FieldOffset(1072)]
        fixed byte l_tls_initimage_size[8];
        [FieldOffset(1080)]
        fixed byte l_tls_blocksize[8];
        [FieldOffset(1088)]
        fixed byte l_tls_align[8];
        [FieldOffset(1096)]
        fixed byte l_tls_firstbyte_offset[8];
        [FieldOffset(1104)]
        fixed byte l_tls_offset[8];
        [FieldOffset(1112)]
        fixed byte l_tls_modid[8];
        [FieldOffset(1120)]
        fixed byte l_tls_dtor_count[8];
        [FieldOffset(1128)]
        fixed byte l_relro_addr[8];
        [FieldOffset(1136)]
        fixed byte l_relro_size[8];
        [FieldOffset(1144)]
        fixed byte l_serial[8];
    }

}
