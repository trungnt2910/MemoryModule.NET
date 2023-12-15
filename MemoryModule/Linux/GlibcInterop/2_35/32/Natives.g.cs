// Native structure offsets for glibc, on x86 and x86_64.
// Generated using GlibcOffsetGen. Do not modify.

using System;
using System.Runtime.InteropServices;
namespace GlibcInterop
{
    [StructLayout(LayoutKind.Explicit, Size = 2348)]
    unsafe partial struct rtld_global_2_35_32
    {
        [FieldOffset(0)]
        fixed byte _dl_ns[1408];
        [FieldOffset(1408)]
        fixed byte _dl_nns[4];
        [FieldOffset(1412)]
        fixed byte _dl_load_lock[24];
        [FieldOffset(1436)]
        fixed byte _dl_load_write_lock[24];
        [FieldOffset(1460)]
        fixed byte _dl_load_tls_lock[24];
        [FieldOffset(1484)]
        fixed byte _dl_load_adds[8];
        [FieldOffset(1492)]
        fixed byte _dl_initfirst[4];
        [FieldOffset(1496)]
        fixed byte _dl_profile_map[4];
        [FieldOffset(1500)]
        fixed byte _dl_num_relocations[4];
        [FieldOffset(1504)]
        fixed byte _dl_num_cache_relocations[4];
        [FieldOffset(1508)]
        fixed byte _dl_all_dirs[4];
        [FieldOffset(1512)]
        fixed byte _dl_rtld_map[624];
        [FieldOffset(2136)]
        fixed byte _dl_rtld_auditstate[128];
        [FieldOffset(2264)]
        fixed byte _dl_x86_feature_1[4];
        [FieldOffset(2268)]
        fixed byte _dl_x86_feature_control[4];
        [FieldOffset(2272)]
        fixed byte _dl_stack_flags[4];
        [FieldOffset(2276)]
        fixed byte _dl_tls_dtv_gaps[1];
        [FieldOffset(2280)]
        fixed byte _dl_tls_max_dtv_idx[4];
        [FieldOffset(2284)]
        fixed byte _dl_tls_dtv_slotinfo_list[4];
        [FieldOffset(2288)]
        fixed byte _dl_tls_static_nelem[4];
        [FieldOffset(2292)]
        fixed byte _dl_tls_static_used[4];
        [FieldOffset(2296)]
        fixed byte _dl_tls_static_optional[4];
        [FieldOffset(2300)]
        fixed byte _dl_initial_dtv[4];
        [FieldOffset(2304)]
        fixed byte _dl_tls_generation[4];
        [FieldOffset(2308)]
        fixed byte _dl_scope_free_list[4];
        [FieldOffset(2312)]
        fixed byte _dl_stack_used[8];
        [FieldOffset(2320)]
        fixed byte _dl_stack_user[8];
        [FieldOffset(2328)]
        fixed byte _dl_stack_cache[8];
        [FieldOffset(2336)]
        fixed byte _dl_stack_cache_actsize[4];
        [FieldOffset(2340)]
        fixed byte _dl_in_flight_stack[4];
        [FieldOffset(2344)]
        fixed byte _dl_stack_cache_lock[4];
    }

    [StructLayout(LayoutKind.Explicit, Size = 624)]
    unsafe partial struct link_map_2_35_32
    {
        [FieldOffset(0)]
        fixed byte l_addr[4];
        [FieldOffset(4)]
        fixed byte l_name[4];
        [FieldOffset(8)]
        fixed byte l_ld[4];
        [FieldOffset(12)]
        fixed byte l_next[4];
        [FieldOffset(16)]
        fixed byte l_prev[4];
        [FieldOffset(20)]
        fixed byte l_real[4];
        [FieldOffset(24)]
        fixed byte l_ns[4];
        [FieldOffset(28)]
        fixed byte l_libname[4];
        [FieldOffset(32)]
        fixed byte l_info[308];
        [FieldOffset(340)]
        fixed byte l_phdr[4];
        [FieldOffset(344)]
        fixed byte l_entry[4];
        [FieldOffset(348)]
        fixed byte l_phnum[2];
        [FieldOffset(350)]
        fixed byte l_ldnum[2];
        [FieldOffset(352)]
        fixed byte l_searchlist[8];
        [FieldOffset(360)]
        fixed byte l_symbolic_searchlist[8];
        [FieldOffset(368)]
        fixed byte l_loader[4];
        [FieldOffset(372)]
        fixed byte l_versions[4];
        [FieldOffset(376)]
        fixed byte l_nversions[4];
        [FieldOffset(380)]
        fixed byte l_nbuckets[4];
        [FieldOffset(384)]
        fixed byte l_gnu_bitmask_idxbits[4];
        [FieldOffset(388)]
        fixed byte l_gnu_shift[4];
        [FieldOffset(392)]
        fixed byte l_gnu_bitmask[4];
        [FieldOffset(396)]
        fixed byte l_gnu_buckets[4];
        [FieldOffset(396)]
        fixed byte l_chain[4];
        [FieldOffset(400)]
        fixed byte l_gnu_chain_zero[4];
        [FieldOffset(400)]
        fixed byte l_buckets[4];
        [FieldOffset(404)]
        fixed byte l_direct_opencount[4];
        [FieldOffset(411)]
        fixed byte l_nodelete_active[1];
        [FieldOffset(412)]
        fixed byte l_nodelete_pending[1];
        [FieldOffset(416)]
        fixed byte l_x86_feature_1_and[4];
        [FieldOffset(420)]
        fixed byte l_x86_isa_1_needed[4];
        [FieldOffset(424)]
        fixed byte l_1_needed[4];
        [FieldOffset(428)]
        fixed byte l_rpath_dirs[8];
        [FieldOffset(436)]
        fixed byte l_reloc_result[4];
        [FieldOffset(440)]
        fixed byte l_versyms[4];
        [FieldOffset(444)]
        fixed byte l_origin[4];
        [FieldOffset(448)]
        fixed byte l_map_start[4];
        [FieldOffset(452)]
        fixed byte l_map_end[4];
        [FieldOffset(456)]
        fixed byte l_text_end[4];
        [FieldOffset(460)]
        fixed byte l_scope_mem[16];
        [FieldOffset(476)]
        fixed byte l_scope_max[4];
        [FieldOffset(484)]
        fixed byte l_local_scope[8];
        [FieldOffset(492)]
        fixed byte l_file_id[16];
        [FieldOffset(508)]
        fixed byte l_runpath_dirs[8];
        [FieldOffset(520)]
        fixed byte l_reldeps[4];
        [FieldOffset(524)]
        fixed byte l_reldepsmax[4];
        [FieldOffset(528)]
        fixed byte l_used[4];
        [FieldOffset(532)]
        fixed byte l_feature_1[4];
        [FieldOffset(536)]
        fixed byte l_flags_1[4];
        [FieldOffset(540)]
        fixed byte l_flags[4];
        [FieldOffset(544)]
        fixed byte l_idx[4];
        [FieldOffset(548)]
        fixed byte l_mach[12];
        [FieldOffset(560)]
        fixed byte l_lookup_cache[16];
        [FieldOffset(576)]
        fixed byte l_tls_initimage[4];
        [FieldOffset(580)]
        fixed byte l_tls_initimage_size[4];
        [FieldOffset(584)]
        fixed byte l_tls_blocksize[4];
        [FieldOffset(588)]
        fixed byte l_tls_align[4];
        [FieldOffset(592)]
        fixed byte l_tls_firstbyte_offset[4];
        [FieldOffset(596)]
        fixed byte l_tls_offset[4];
        [FieldOffset(600)]
        fixed byte l_tls_modid[4];
        [FieldOffset(604)]
        fixed byte l_tls_dtor_count[4];
        [FieldOffset(608)]
        fixed byte l_relro_addr[4];
        [FieldOffset(612)]
        fixed byte l_relro_size[4];
        [FieldOffset(616)]
        fixed byte l_serial[8];
    }

}
