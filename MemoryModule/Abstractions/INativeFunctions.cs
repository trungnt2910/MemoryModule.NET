using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Abstractions
{
    /// <summary>
    /// Provides low-level functions, such as memory mapping and module loading.
    /// </summary>
    public interface INativeFunctions
    {
        /// <summary>
        /// Loads a native library from a file.
        /// </summary>
        /// <param name="name">The library's path.</param>
        /// <returns>A handle to the library.</returns>
        IntPtr LoadLibrary(string name);
        /// <summary>
        /// Loads an exported symbol from a library.
        /// </summary>
        /// <param name="handle">The library's handle, as provided by <see cref="LoadLibrary(string)"/></param>
        /// <param name="name">The symbol's name.</param>
        /// <returns>The address of the symbol.</returns>
        IntPtr GetSymbolFromLibrary(IntPtr handle, string name);
        /// <summary>
        /// Loads an exported symbol from a library.
        /// </summary>
        /// <param name="handle">The library's handle, as provided by <see cref="LoadLibrary(string)"/></param>
        /// <param name="nameValue">Either a pointer to the symbol's string, or the index of the symbol in
        /// the module's export table.</param>
        /// <returns>The address of the symbol.</returns>
        IntPtr GetSymbolFromLibrary(IntPtr handle, IntPtr nameValue);
        /// <summary>
        /// Frees a native library.
        /// </summary>
        /// <param name="handle">The library's handle</param>
        /// <returns>A boolean, indicating whether the operation succeeded.</returns>
        bool FreeLibrary(IntPtr handle);
        /// <summary>
        /// Allocates a block of memory from the system pages. The memory should be zero-inited.
        /// </summary>
        /// <param name="hint">A hinted address. The system should allocate here if possible.</param>
        /// <param name="size">The size of the allocated memory.</param>
        /// <param name="protection">The protection of the newly allocated pages.</param>
        /// <returns>The newly allocated memory.</returns>
        IntPtr VirtualAllocate(IntPtr hint, ulong size, MemoryProtection protection);
        /// <summary>
        /// Changes the protection of a range of pages. <paramref name="addr"/> and <paramref name="size"/> must be a multiple
        /// of the system page size.
        /// </summary>
        /// <param name="addr">The starting address.</param>
        /// <param name="size">The size of the protected memory, in bytes.</param>
        /// <param name="protection">The new protection.</param>
        /// <returns>A boolean, indicating whether the operation succeeded.</returns>
        bool VirtualProtect(IntPtr addr, ulong size, MemoryProtection protection);
        /// <summary>
        /// Frees a region of memory allocated by <see cref="VirtualAllocate(IntPtr, ulong, MemoryProtection)"/>.
        /// </summary>
        /// <param name="addr">The address of the allocated region.</param>
        /// <param name="size">The size of the allocated region. Must be the original size requested 
        /// in <see cref="VirtualAllocate(IntPtr, ulong, MemoryProtection)">, else the behavior is undefined.</param>
        /// <returns>A boolean, indicating whether the operation succeded.</returns>
        bool VirtualFree(IntPtr addr, ulong size);
        /// <summary>
        /// Fills a region of <paramref name="size"/> bytes, starting from <paramref name="src"/>, with the byte specified in <paramref name="ch"/>.
        /// </summary>
        /// <param name="dest">The destination address</param>
        /// <param name="size">Number of bytes to fill</param>
        /// <param name="ch">The bytes filled</param>
        /// <returns>The destination address</returns>
        IntPtr FillMemory(IntPtr dest, byte ch, ulong size);
        /// <summary>
        /// Copies a region of <paramref name="size"/> bytes, from <paramref name="src"/> to <paramref name="dest"/> 
        /// </summary>
        /// <param name="dest">The destination memory</param>
        /// <param name="src">The source memory</param>
        /// <param name="size">The size of the block of memory to copy</param>
        /// <returns><paramref name="dest"/></returns>
        IntPtr CopyMemory(IntPtr dest, IntPtr src, ulong size);
    }
}
