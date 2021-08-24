using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule
{
    internal static unsafe class CStyleMemory
    {
        private const int magic = 0x414d_534c;

        public static unsafe void* calloc(uint nitems, uint count)
        {
            uint size = nitems * count;
            void* voidPtr = malloc(size);
            if (voidPtr != null)
            {
                byte* numPtr = (byte*)voidPtr;
                int index = 0;
                while (true)
                {
                    if (index >= size)
                    {
                        break;
                    }
                    numPtr[index] = 0;
                    index++;
                }
            }
            return voidPtr;
        }

        public static unsafe void free(void* item)
        {
            if ((item != null) && mem_is_valid(item))
            {
                item = (void *)IntPtr.Subtract((IntPtr)item, 8);
                Marshal.FreeHGlobal(new IntPtr(item));
            }
        }

        public static unsafe void* malloc(uint size)
        {
            void* voidPtr = Marshal.AllocHGlobal((int)(((int)size) + 8)).ToPointer();
            if (voidPtr != null)
            {
                *((int*)voidPtr) = 0x414d_534c;
                *((uint*)(IntPtr.Add((IntPtr)voidPtr, 4))) = size;
                voidPtr = (void *)IntPtr.Add((IntPtr)voidPtr, 8);
            }
            return voidPtr;
        }

        private static unsafe bool mem_is_valid(void* item)
        {
            item = (void*)IntPtr.Subtract((IntPtr)item, 8);
            return (*(((int*)item)) == 0x414d_534c);
        }

        public static unsafe void* realloc(void* item, uint size)
        {
            void* voidPtr;
            if (item == null)
            {
                voidPtr = malloc(size);
            }
            else if (size == 0)
            {
                free(item);
                voidPtr = null;
            }
            else if (!mem_is_valid(item))
            {
                voidPtr = null;
            }
            else
            {
                uint num = *((uint*)(IntPtr.Subtract((IntPtr)item, 4)));
                if (size < num)
                {
                    num = size;
                }
                void* voidPtr2 = malloc(size);
                if (voidPtr2 != null)
                {
                    int num2 = 0;
                    while (true)
                    {
                        if (num2 >= num)
                        {
                            break;
                        }
                        *((byte*)(IntPtr.Add((IntPtr)voidPtr2, num2))) = *((byte*)(IntPtr.Add((IntPtr)item, num2)));
                        num2++;
                    }
                }
                free(item);
                voidPtr = voidPtr2;
            }
            return voidPtr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* memcpy(void* __dest, void* __src, uint __n)
        {
            Unsafe.CopyBlockUnaligned(__dest, __src, __n);
            return __dest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* memset(void* __s, int __c, uint __n)
        {
            Unsafe.InitBlockUnaligned(__s, (byte)__c, __n);
            return __s;
        }
    }
}
