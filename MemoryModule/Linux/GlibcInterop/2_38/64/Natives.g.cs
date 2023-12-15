// Native structure offsets for glibc, on x86 and x86_64.
// Generated using GlibcOffsetGen. Do not modify.

using System;
using System.Runtime.InteropServices;
namespace GlibcInterop
{
    [StructLayout(LayoutKind.Explicit, Size = 4328)]
    unsafe partial struct rtld_global_2_38_64
    {
        [FieldOffset(0)]
        fixed byte _dl_ns[2560];
        [FieldOffset(2560)]
        fixed byte _dl_nns[8];
        [FieldOffset(2568)]
        fixed byte _dl_load_lock[40];
        [FieldOffset(2608)]
        fixed byte _dl_load_write_lock[40];
        [FieldOffset(2648)]
        fixed byte _dl_load_tls_lock[40];
        [FieldOffset(2688)]
        fixed byte _dl_load_adds[8];
        [FieldOffset(2696)]
        fixed byte _dl_initfirst[8];
        [FieldOffset(2704)]
        fixed byte _dl_profile_map[8];
        [FieldOffset(2712)]
        fixed byte _dl_num_relocations[8];
        [FieldOffset(2720)]
        fixed byte _dl_num_cache_relocations[8];
        [FieldOffset(2728)]
        fixed byte _dl_all_dirs[8];
        [FieldOffset(2736)]
        fixed byte _dl_rtld_map[1184];
        [FieldOffset(3920)]
        fixed byte _dl_rtld_auditstate[256];
        [FieldOffset(4176)]
        fixed byte _dl_x86_feature_1[4];
        [FieldOffset(4180)]
        fixed byte _dl_x86_feature_control[4];
        [FieldOffset(4184)]
        fixed byte _dl_stack_flags[4];
        [FieldOffset(4188)]
        fixed byte _dl_tls_dtv_gaps[1];
        [FieldOffset(4192)]
        fixed byte _dl_tls_max_dtv_idx[8];
        [FieldOffset(4200)]
        fixed byte _dl_tls_dtv_slotinfo_list[8];
        [FieldOffset(4208)]
        fixed byte _dl_tls_static_nelem[8];
        [FieldOffset(4216)]
        fixed byte _dl_tls_static_used[8];
        [FieldOffset(4224)]
        fixed byte _dl_tls_static_optional[8];
        [FieldOffset(4232)]
        fixed byte _dl_initial_dtv[8];
        [FieldOffset(4240)]
        fixed byte _dl_tls_generation[8];
        [FieldOffset(4248)]
        fixed byte _dl_scope_free_list[8];
        [FieldOffset(4256)]
        fixed byte _dl_stack_used[16];
        [FieldOffset(4272)]
        fixed byte _dl_stack_user[16];
        [FieldOffset(4288)]
        fixed byte _dl_stack_cache[16];
        [FieldOffset(4304)]
        fixed byte _dl_stack_cache_actsize[8];
        [FieldOffset(4312)]
        fixed byte _dl_in_flight_stack[8];
        [FieldOffset(4320)]
        fixed byte _dl_stack_cache_lock[4];
    }

    [StructLayout(LayoutKind.Explicit, Size = 1184)]
    unsafe partial struct link_map_2_38_64
    {
        [FieldOffset(0)]
        fixed byte l_addr[8];
        [FieldOffset(8)]
        fixed byte l_name[8];
        [FieldOffset(16)]
        fixed byte l_ld[8];
        [FieldOffset(24)]
        fixed byte l_next[8];
        [FieldOffset(32)]
        fixed byte l_prev[8];
        [FieldOffset(40)]
        fixed byte l_real[8];
        [FieldOffset(48)]
        fixed byte l_ns[8];
        [FieldOffset(56)]
        fixed byte l_libname[8];
        [FieldOffset(64)]
        fixed byte l_info[640];
        [FieldOffset(704)]
        fixed byte l_phdr[8];
        [FieldOffset(712)]
        fixed byte l_entry[8];
        [FieldOffset(720)]
        fixed byte l_phnum[2];
        [FieldOffset(722)]
        fixed byte l_ldnum[2];
        [FieldOffset(728)]
        fixed byte l_searchlist[16];
        [FieldOffset(744)]
        fixed byte l_symbolic_searchlist[16];
        [FieldOffset(760)]
        fixed byte l_loader[8];
        [FieldOffset(768)]
        fixed byte l_versions[8];
        [FieldOffset(776)]
        fixed byte l_nversions[4];
        [FieldOffset(780)]
        fixed byte l_nbuckets[4];
        [FieldOffset(784)]
        fixed byte l_gnu_bitmask_idxbits[4];
        [FieldOffset(788)]
        fixed byte l_gnu_shift[4];
        [FieldOffset(792)]
        fixed byte l_gnu_bitmask[8];
        [FieldOffset(800)]
        fixed byte l_gnu_buckets[8];
        [FieldOffset(800)]
        fixed byte l_chain[8];
        [FieldOffset(808)]
        fixed byte l_gnu_chain_zero[8];
        [FieldOffset(808)]
        fixed byte l_buckets[8];
        [FieldOffset(816)]
        fixed byte l_direct_opencount[4];
        [FieldOffset(823)]
        fixed byte l_nodelete_active[1];
        [FieldOffset(824)]
        fixed byte l_nodelete_pending[1];
        [FieldOffset(828)]
        fixed byte l_x86_feature_1_and[4];
        [FieldOffset(832)]
        fixed byte l_x86_isa_1_needed[4];
        [FieldOffset(836)]
        fixed byte l_1_needed[4];
        [FieldOffset(840)]
        fixed byte l_rpath_dirs[16];
        [FieldOffset(856)]
        fixed byte l_reloc_result[8];
        [FieldOffset(864)]
        fixed byte l_versyms[8];
        [FieldOffset(872)]
        fixed byte l_origin[8];
        [FieldOffset(880)]
        fixed byte l_map_start[8];
        [FieldOffset(888)]
        fixed byte l_map_end[8];
        [FieldOffset(896)]
        fixed byte l_text_end[8];
        [FieldOffset(904)]
        fixed byte l_scope_mem[32];
        [FieldOffset(936)]
        fixed byte l_scope_max[8];
        [FieldOffset(952)]
        fixed byte l_local_scope[16];
        [FieldOffset(968)]
        fixed byte l_file_id[16];
        [FieldOffset(984)]
        fixed byte l_runpath_dirs[16];
        [FieldOffset(1008)]
        fixed byte l_reldeps[8];
        [FieldOffset(1016)]
        fixed byte l_reldepsmax[4];
        [FieldOffset(1020)]
        fixed byte l_used[4];
        [FieldOffset(1024)]
        fixed byte l_feature_1[4];
        [FieldOffset(1028)]
        fixed byte l_flags_1[4];
        [FieldOffset(1032)]
        fixed byte l_flags[4];
        [FieldOffset(1036)]
        fixed byte l_idx[4];
        [FieldOffset(1040)]
        fixed byte l_mach[24];
        [FieldOffset(1064)]
        fixed byte l_lookup_cache[32];
        [FieldOffset(1096)]
        fixed byte l_tls_initimage[8];
        [FieldOffset(1104)]
        fixed byte l_tls_initimage_size[8];
        [FieldOffset(1112)]
        fixed byte l_tls_blocksize[8];
        [FieldOffset(1120)]
        fixed byte l_tls_align[8];
        [FieldOffset(1128)]
        fixed byte l_tls_firstbyte_offset[8];
        [FieldOffset(1136)]
        fixed byte l_tls_offset[8];
        [FieldOffset(1144)]
        fixed byte l_tls_modid[8];
        [FieldOffset(1152)]
        fixed byte l_tls_dtor_count[8];
        [FieldOffset(1160)]
        fixed byte l_relro_addr[8];
        [FieldOffset(1168)]
        fixed byte l_relro_size[8];
        [FieldOffset(1176)]
        fixed byte l_serial[8];
    }

}
