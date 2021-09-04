using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLibcInterop
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct pthread_mutex_t
    {
        [FieldOffset(0)]
        public __pthread_mutex_s_32 __data32;
        [FieldOffset(0)]
        public __pthread_mutex_s_64 __data64;
        [FieldOffset(0)]
        IntPtr __align;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct __pthread_mutex_s_32
    {
        public int __lock;
        public uint __count;
        public int __owner;
        public int __kind;
        public uint __nusers;

        [StructLayout(LayoutKind.Explicit)]
        public struct DummyUnion
        {
            public struct __elision_data_t
            {
                public short __espins;
                public short __eelision;
            }
            [FieldOffset(0)]
            public __elision_data_t __elision_data;
            [FieldOffset(0)]
            public __pthread_slist_t __list;
        }

        public DummyUnion unionData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct __pthread_mutex_s_64
    {
        // On 64-bit platforms, these two eat up 16 bytes.
        // On 32-bit, they only take 8.
        public IntPtr __lock_and_count;
        public IntPtr __owner_and_nusers;

        public int __kind;

        public short __spins;
        public short __elision;

        // 16 bytes on 64, 8 bytes on 32 bit.
        public __pthread_list_t __list;
    };


    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct __pthread_slist_t
    {
        __pthread_slist_t *__next;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct __pthread_list_t
    {
        public __pthread_list_t * __prev;
        public __pthread_list_t * __next;
    }
}
