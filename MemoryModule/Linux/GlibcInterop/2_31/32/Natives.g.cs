// Native structure offsets for glibc, on x86 and x86_64.
// Generated using GlibcOffsetGen. Do not modify.

using System;
using System.Runtime.InteropServices;
namespace GlibcInterop
{
    [StructLayout(LayoutKind.Explicit, Size = 2180)]
    unsafe partial struct rtld_global_2_31_32
    {
        [FieldOffset(0)]
        fixed byte _dl_ns[1280];
        [FieldOffset(1280)]
        fixed byte _dl_nns[4];
        [FieldOffset(1284)]
        fixed byte _dl_load_lock[24];
        [FieldOffset(1308)]
        fixed byte _dl_load_write_lock[24];
        [FieldOffset(1332)]
        fixed byte _dl_load_adds[8];
        [FieldOffset(1340)]
        fixed byte _dl_initfirst[4];
        [FieldOffset(1344)]
        fixed byte _dl_profile_map[4];
        [FieldOffset(1348)]
        fixed byte _dl_num_relocations[4];
        [FieldOffset(1352)]
        fixed byte _dl_num_cache_relocations[4];
        [FieldOffset(1356)]
        fixed byte _dl_all_dirs[4];
        [FieldOffset(1360)]
        fixed byte _dl_rtld_map[612];
        [FieldOffset(1972)]
        fixed byte _dl_rtld_auditstate[128];
        [FieldOffset(2108)]
        fixed byte _dl_x86_feature_1[8];
        [FieldOffset(2116)]
        fixed byte _dl_x86_legacy_bitmap[8];
        [FieldOffset(2128)]
        fixed byte _dl_stack_flags[4];
        [FieldOffset(2132)]
        fixed byte _dl_tls_dtv_gaps[1];
        [FieldOffset(2136)]
        fixed byte _dl_tls_max_dtv_idx[4];
        [FieldOffset(2140)]
        fixed byte _dl_tls_dtv_slotinfo_list[4];
        [FieldOffset(2144)]
        fixed byte _dl_tls_static_nelem[4];
        [FieldOffset(2148)]
        fixed byte _dl_tls_static_size[4];
        [FieldOffset(2152)]
        fixed byte _dl_tls_static_used[4];
        [FieldOffset(2156)]
        fixed byte _dl_tls_static_align[4];
        [FieldOffset(2160)]
        fixed byte _dl_initial_dtv[4];
        [FieldOffset(2164)]
        fixed byte _dl_tls_generation[4];
        [FieldOffset(2176)]
        fixed byte _dl_scope_free_list[4];
    }

    [StructLayout(LayoutKind.Explicit, Size = 612)]
    unsafe partial struct link_map_2_31_32
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
        fixed byte l_rpath_dirs[8];
        [FieldOffset(424)]
        fixed byte l_reloc_result[4];
        [FieldOffset(428)]
        fixed byte l_versyms[4];
        [FieldOffset(432)]
        fixed byte l_origin[4];
        [FieldOffset(436)]
        fixed byte l_map_start[4];
        [FieldOffset(440)]
        fixed byte l_map_end[4];
        [FieldOffset(444)]
        fixed byte l_text_end[4];
        [FieldOffset(448)]
        fixed byte l_scope_mem[16];
        [FieldOffset(464)]
        fixed byte l_scope_max[4];
        [FieldOffset(472)]
        fixed byte l_local_scope[8];
        [FieldOffset(480)]
        fixed byte l_file_id[16];
        [FieldOffset(496)]
        fixed byte l_runpath_dirs[8];
        [FieldOffset(508)]
        fixed byte l_reldeps[4];
        [FieldOffset(512)]
        fixed byte l_reldepsmax[4];
        [FieldOffset(516)]
        fixed byte l_used[4];
        [FieldOffset(520)]
        fixed byte l_feature_1[4];
        [FieldOffset(524)]
        fixed byte l_flags_1[4];
        [FieldOffset(528)]
        fixed byte l_flags[4];
        [FieldOffset(532)]
        fixed byte l_idx[4];
        [FieldOffset(536)]
        fixed byte l_mach[12];
        [FieldOffset(548)]
        fixed byte l_lookup_cache[16];
        [FieldOffset(564)]
        fixed byte l_tls_initimage[4];
        [FieldOffset(568)]
        fixed byte l_tls_initimage_size[4];
        [FieldOffset(572)]
        fixed byte l_tls_blocksize[4];
        [FieldOffset(576)]
        fixed byte l_tls_align[4];
        [FieldOffset(580)]
        fixed byte l_tls_firstbyte_offset[4];
        [FieldOffset(584)]
        fixed byte l_tls_offset[4];
        [FieldOffset(588)]
        fixed byte l_tls_modid[4];
        [FieldOffset(592)]
        fixed byte l_tls_dtor_count[4];
        [FieldOffset(596)]
        fixed byte l_relro_addr[4];
        [FieldOffset(600)]
        fixed byte l_relro_size[4];
        [FieldOffset(604)]
        fixed byte l_serial[8];
    }

}
