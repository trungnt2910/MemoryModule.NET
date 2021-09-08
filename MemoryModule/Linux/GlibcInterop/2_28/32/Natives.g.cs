// Native structure offsets for glibc, on x86 and x86_64.
// Generated using GlibcOffsetGen. Do not modify.

using System;
using System.Runtime.InteropServices;
namespace GlibcInterop
{
    [StructLayout(LayoutKind.Explicit, Size = 2120)]
    unsafe partial struct rtld_global_2_28_32
    {
        [FieldOffset(0)]
        fixed byte _dl_ns[1216];
        [FieldOffset(1216)]
        fixed byte _dl_nns[4];
        [FieldOffset(1220)]
        fixed byte _dl_load_lock[24];
        [FieldOffset(1244)]
        fixed byte _dl_load_write_lock[24];
        [FieldOffset(1268)]
        fixed byte _dl_load_adds[8];
        [FieldOffset(1280)]
        fixed byte _dl_cpuclock_offset[8];
        [FieldOffset(1292)]
        fixed byte _dl_num_relocations[4];
        [FieldOffset(1296)]
        fixed byte _dl_num_cache_relocations[4];
        [FieldOffset(1304)]
        fixed byte _dl_rtld_map[608];
        [FieldOffset(2048)]
        fixed byte _dl_x86_feature_1[8];
        [FieldOffset(2056)]
        fixed byte _dl_x86_legacy_bitmap[8];
        [FieldOffset(2068)]
        fixed byte _dl_stack_flags[4];
        [FieldOffset(2072)]
        fixed byte _dl_tls_dtv_gaps[1];
        [FieldOffset(2076)]
        fixed byte _dl_tls_max_dtv_idx[4];
        [FieldOffset(2084)]
        fixed byte _dl_tls_static_nelem[4];
        [FieldOffset(2088)]
        fixed byte _dl_tls_static_size[4];
        [FieldOffset(2092)]
        fixed byte _dl_tls_static_used[4];
        [FieldOffset(2096)]
        fixed byte _dl_tls_static_align[4];
        [FieldOffset(2104)]
        fixed byte _dl_tls_generation[4];
    }

    [StructLayout(LayoutKind.Explicit, Size = 608)]
    unsafe partial struct link_map_2_28_32
    {
        [FieldOffset(0)]
        fixed byte l_addr[4];
        [FieldOffset(24)]
        fixed byte l_ns[4];
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
        [FieldOffset(376)]
        fixed byte l_nversions[4];
        [FieldOffset(380)]
        fixed byte l_nbuckets[4];
        [FieldOffset(384)]
        fixed byte l_gnu_bitmask_idxbits[4];
        [FieldOffset(388)]
        fixed byte l_gnu_shift[4];
        [FieldOffset(404)]
        fixed byte l_direct_opencount[4];
        [FieldOffset(412)]
        fixed byte l_rpath_dirs[8];
        [FieldOffset(432)]
        fixed byte l_map_start[4];
        [FieldOffset(436)]
        fixed byte l_map_end[4];
        [FieldOffset(440)]
        fixed byte l_text_end[4];
        [FieldOffset(460)]
        fixed byte l_scope_max[4];
        [FieldOffset(476)]
        fixed byte l_file_id[16];
        [FieldOffset(492)]
        fixed byte l_runpath_dirs[8];
        [FieldOffset(508)]
        fixed byte l_reldepsmax[4];
        [FieldOffset(512)]
        fixed byte l_used[4];
        [FieldOffset(516)]
        fixed byte l_feature_1[4];
        [FieldOffset(520)]
        fixed byte l_flags_1[4];
        [FieldOffset(524)]
        fixed byte l_flags[4];
        [FieldOffset(528)]
        fixed byte l_idx[4];
        [FieldOffset(532)]
        fixed byte l_mach[12];
        [FieldOffset(544)]
        fixed byte l_lookup_cache[16];
        [FieldOffset(564)]
        fixed byte l_tls_initimage_size[4];
        [FieldOffset(568)]
        fixed byte l_tls_blocksize[4];
        [FieldOffset(572)]
        fixed byte l_tls_align[4];
        [FieldOffset(576)]
        fixed byte l_tls_firstbyte_offset[4];
        [FieldOffset(580)]
        fixed byte l_tls_offset[4];
        [FieldOffset(584)]
        fixed byte l_tls_modid[4];
        [FieldOffset(588)]
        fixed byte l_tls_dtor_count[4];
        [FieldOffset(592)]
        fixed byte l_relro_addr[4];
        [FieldOffset(596)]
        fixed byte l_relro_size[4];
        [FieldOffset(600)]
        fixed byte l_serial[8];
        byte* l_audit
        {
            get
            {
                fixed (void* _thisPtr = &this)
                {
                    return (byte*)_thisPtr + 608;
                }
            }
        }
    }

}
