using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    public unsafe class DtvSlotInfoList : IEnumerable<DtvSlotInfoArray>
    {
        DtvSlotInfoListNative* _head;

        public IntPtr Head => (IntPtr)_head;

        public DtvSlotInfoList(DtvSlotInfoListNative* head)
        {
            _head = head;
        }

        public IEnumerator<DtvSlotInfoArray> GetEnumerator()
        {
            var current = Head;
            // Remember, we're using fucking IntPtr's here.
            while (current != IntPtr.Zero)
            {
                yield return new DtvSlotInfoArray(current);
                current = GetNextNode(current);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Workaround for unsafe code in iterators.
        public static IntPtr GetNextNode(IntPtr current)
        {
            var truePtr = (DtvSlotInfoListNative*)current;
            return (IntPtr)truePtr->next;
        }

        public DtvSlotInfo FindElement(ulong offset)
        {
            var current = _head;
            var last = _head;

            while (current != null)
            {
                Console.WriteLine($"0x{(ulong)current:x}");
                Console.WriteLine($"0x{(ulong)current->next}");
                Console.WriteLine($"{current->len}");

                var arr = (DtvSlotInfoNative *)((byte *)current + sizeof(DtvSlotInfoListNative));

                if (offset >= (ulong)current->len)
                {
                    offset -= (ulong)current->len;
                }
                else
                {
                    return new DtvSlotInfo(&arr[offset]);
                }
                last = current;
                current = current->next;
            }

            // When we get here, this means that we need to allocate a new entry.
            // We don't know what the fuck glibc expects, it's something like TLS_SLOTINFO_SURPLUS,
            // but we'll just copy the last entry LMAO.

            var nextLength = Math.Max(offset, (ulong)last->len);

            Console.WriteLine(offset);
            Console.WriteLine(nextLength);

            int slotInfoBlockLength =
                    // A few bytes for the struct
                    sizeof(DtvSlotInfoListNative) +
                    // A few more for the variable lengthed array next to it.
                    sizeof(DtvSlotInfoNative) * (int)nextLength;

            current = 
                (DtvSlotInfoListNative*)Marshal.AllocHGlobal(slotInfoBlockLength);

            Unsafe.InitBlockUnaligned(current, 0, (uint)slotInfoBlockLength);

            current->len = (UIntPtr)nextLength;
            current->next = null;

            last->next = current;

            // current + 1 points to the first byte after that struct,
            // which is the array we want.
            var nextArr = (DtvSlotInfoNative *)(current + 1);

            return new DtvSlotInfo(&nextArr[offset]);
        }
    }
}
