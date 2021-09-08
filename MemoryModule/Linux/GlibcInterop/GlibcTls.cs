using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GlibcInterop
{
    public static unsafe class GlibcTls
    {
        private static readonly RtldGlobal _global;

        private static readonly Version[] TestedVersions =
            new Version[]
        {
            new Version(2, 33),
        };

        public static readonly Version Version;

        /// <summary>
        /// A naive implementation _dl_next_tls_modid,
        /// but free from GNU Communistware.
        /// </summary>
        /// <returns>The next module id, according to glibc's rule</returns>
        public static ulong GetNextModuleId()
        {
            // Mostly, there won't be any gaps. Return the next module index.
            if (!_global.TlsDtvHasGaps)
            {
                return ++_global.TlsDtvMaxIndex;
            }
            else
            {
                // The static modules should stay there, always.
                // The index 0 is used as a generation counter.
                var result = _global.TlsStaticElementCount + 1;
                var offset = 0ul;

                foreach (var item in _global.TlsDtvSlotInfoList)
                {
                    for (ulong i = result - offset; i < item.Count; ++i, ++result)
                    {
                        // There is a position without a Link Map - use it.
                        if (item[i].LinkMapPtr == null)
                        {
                            goto bail;
                        }
                    }
                    offset += item.Count;
                }

                bail:
                Debug.Assert(result <= _global.TlsDtvMaxIndex + 1);
                
                // There are actually no gaps!
                if (result == _global.TlsDtvMaxIndex + 1)
                {
                    _global.TlsDtvHasGaps = false;
                    ++_global.TlsDtvMaxIndex;
                }

                return result;
            }
        }

        /// <summary>
        /// A naive implementation of _dl_add_to_slotinfo
        /// </summary>
        /// <param name="l">The link map that needs to be added</param>
        public static void AddToSlotInfo(LinkMap l)
        {
            ulong pos = l.TlsModuleId;

            var elem = _global.TlsDtvSlotInfoList.FindElement(pos);

            elem.LinkMapPtr = l.GetNativePointer();
            elem.Generation = ++_global.TlsGeneration;
        }

        static GlibcTls()
        {
            _global = RtldGlobal.Instance;

            try
            {
                _dl_get_tls_static_info(out var size, out var align);
                if ((ulong)size != _global.TlsStaticSize || (ulong)align != _global.TlsStaticAlign)
                {
                    var message = "rtld_global data seems to be corrupted.\n" +
                        "Please open an issue at github.com/trungnt2910/MemoryModule.NET";

                    Console.WriteLine(message);
                    Debug.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                var message = $"Failed to verify rtld_global: {e}\n" +
                    "Please open an issue at github.com/trungnt2910/MemoryModule.NET";

                Console.WriteLine(message);
                Debug.WriteLine(message);

            }
        }

        [DllImport("libc")]
        private static extern void _dl_get_tls_static_info(out UIntPtr size, out UIntPtr align);
    }
}
