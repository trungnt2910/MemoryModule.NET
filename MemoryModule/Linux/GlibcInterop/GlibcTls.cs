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

        /// <summary>
        /// A naive implementation of remove_slotinfo
        /// </summary>
        /// <param name="l">The link map that should be removed, that has been added using AddToSlotInfo</param>
        public static void RemoveFromSlotInfo(LinkMap l)
        {
            RemoveFromSlotInfoInternal(l, (DtvSlotInfoListNative*)_global.TlsDtvSlotInfoList.Head, 0);
        }

        private static bool RemoveFromSlotInfoInternal(LinkMap l, DtvSlotInfoListNative* head, ulong basePos)
        {
            ulong index;

            var managedHead = new DtvSlotInfoList(head);
            var arr = managedHead.First();

            if (l.TlsModuleId >= basePos + (ulong)head->len)
            {
                if (head->next == null)
                {
                    throw new InvalidOperationException("Invalid link map index.");
                }

                if (RemoveFromSlotInfoInternal(l, head->next, basePos + (ulong)head->len))
                {
                    return true;
                }

                if ((ulong)head->len == 0)
                {
                    return false;
                }

                index = (ulong)head->len - 1; 
            }
            else
            {
                var elem = arr[l.TlsModuleId - basePos];
                elem.Generation = ++_global.TlsGeneration;

                Debug.Assert(elem.LinkMapPtr == l.GetNativePointer());
                elem.LinkMapPtr = null;

                if (l.TlsModuleId != _global.TlsDtvMaxIndex)
                {
                    // Lots of other elems to the right.
                    return true;
                }

                index = l.TlsModuleId - 1;
            }

            ulong stop = (basePos == 0) ? _global.TlsStaticElementCount + 1 : 0;

            while (true)
            {
                if (arr[index].LinkMapPtr != null)
                {
                    // Shrink!
                    _global.TlsDtvMaxIndex = basePos + index;
                    return true;
                }

                --index;

                if (index == stop)
                {
                    return false;
                }
            }
        }

        static GlibcTls()
        {
            _global = RtldGlobal.Instance;
        }
    }
}
