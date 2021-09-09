using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal unsafe delegate void* CustomAllocFunc(void* ptr, UIntPtr size, MemoryAllocation a, PageProtection b, void* ptr1);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal unsafe delegate bool CustomFreeFunc(void* ptr, UIntPtr size, MemoryAllocation a, void* ptr1);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal unsafe delegate void* CustomLoadLibraryFunc(char* str, void* ptr);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal unsafe delegate void* CustomGetProcAddressFunc(void* customModule, char* name, void* ptr);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal unsafe delegate bool CustomFreeLibraryFunc(void* customModule, void* ptr);
}
