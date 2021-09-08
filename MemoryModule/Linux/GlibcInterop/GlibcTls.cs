using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GlibcInterop
{
    internal static unsafe class GlibcTls
    {
        private static readonly RtldGlobal _global;

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
        }
    }
}
