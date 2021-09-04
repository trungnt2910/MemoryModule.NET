using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GLibcInterop
{
    public static unsafe class GlibcTls
    {
        /// <summary>
        /// A naive implementation _dl_next_tls_modid,
        /// but free from GNU Communistware.
        /// </summary>
        /// <returns>The next module id, according to glibc's rule</returns>
        public static ulong GetNextModuleId()
        {
            var global = RtldGlobal.GetInstance();
            
            // Mostly, there won't be any gaps. Return the next module index.
            if (!global->TlsDtvHasGaps)
            {
                return ++global->TlsDtvMaxIndex;
            }
            else
            {
                // The static modules should stay there, always.
                // The index 0 is used as a generation counter.
                var result = (ulong)global->_dl_tls_static_nelem + 1;
                var offset = 0ul;

                foreach (var item in global->TlsDtvSlotInfoList)
                {
                    for (ulong i = result - offset; i < item.Count; ++i, ++result)
                    {
                        // There is a position without a Link Map - use it.
                        if (item[i].LinkMap == null)
                        {
                            goto bail;
                        }
                    }
                    offset += item.Count;
                }

                bail:
                Debug.Assert(result <= global->TlsDtvMaxIndex + 1);
                
                // There are actually no gaps!
                if (result == global->TlsDtvMaxIndex + 1)
                {
                    global->TlsDtvHasGaps = false;
                    ++global->TlsDtvMaxIndex;
                }

                return result;
            }
        }

        /// <summary>
        /// A naive implementation of _dl_add_to_slotinfo
        /// </summary>
        /// <param name="l">The link map that needs to be added</param>
        public static void AddToSlotInfo(LinkMap* l)
        {
            var global = RtldGlobal.GetInstance();

            ulong pos = l->TlsModuleId;

            var elem = global->TlsDtvSlotInfoList.FindElement(pos);

            elem.LinkMap = l;
            elem.Generation = ++global->TlsGeneration;
        }
    }
}
