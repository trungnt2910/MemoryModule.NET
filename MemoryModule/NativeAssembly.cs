using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace MemoryModule
{
    public class NativeAssembly : IDisposable
    {
        private class LibraryInfo
        {
            public uint RefCount;
            public string Name;

            public LibraryInfo(string name)
            {
                RefCount = 1;
                Name = name;
            }
        }
        
        private static readonly Dictionary<IntPtr, LibraryInfo> _handles = new Dictionary<IntPtr, LibraryInfo>();
        // Windows library names are case-insensitive, right?
        private static readonly Dictionary<string, HashSet<IntPtr>> _libraryMap
            = new Dictionary<string, HashSet<IntPtr>>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly unsafe
            NativeAssemblyImpl.CustomGetProcAddressFunc GetProcAddressDelegate = GetProcAddressUnsafe;
        private static readonly unsafe
            NativeAssemblyImpl.CustomFreeLibraryFunc FreeLibraryDelegate = FreeLibraryUnsafe;

        private IntPtr _handle;
        private string _name;
        private bool _disposedValue;
        private readonly unsafe
            NativeAssemblyImpl.CustomLoadLibraryFunc LoadLibraryDelegate;


        private unsafe NativeAssembly()
        {
            LoadLibraryDelegate = LoadLibraryUnsafe;
        }

        /// <summary>
        /// The user-specified name for this NativeAssembly.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Loads a NativeAssembly, from the specified memory region.
        /// </summary>
        /// <param name="data">The byte[] containing the assembly.</param>
        /// <param name="name">The name this assembly should be associated with
        /// for dependency resolution. Can be null, to exclude from resolution.</param>
        /// <returns>A new NativeAssembly</returns>
        public static NativeAssembly Load(byte[] data, string name = null)
        {
            unsafe
            {
                fixed (byte* dataPtr = &data[0])
                {
                    return LoadInternal(dataPtr, data.Length, name);
                }
            }
        }

        /// <summary>
        /// Loads a NativeAssembly, from the specified Stream.
        /// This function does not dispose the given stream.
        /// </summary>
        /// <param name="data">A valid Stream containing the assembly</param>
        /// <param name="name">The name this assembly should be associated with
        /// for dependency resolution. Can be null, to exclude from resolution.</param>
        /// <returns>A new NativeAssembly</returns>
        public static NativeAssembly Load(Stream data, string name = null)
        {
            if (data is MemoryStream mstream)
            {
                // Avoids more copying...
                return Load(mstream.ToArray(), name);
            }
            // The most common stream in resoure files.
            else if (data is UnmanagedMemoryStream umstream)
            {
                unsafe
                {
                    try
                    {
                        return LoadInternal(
                           umstream.PositionPointer,
                           umstream.Length - umstream.Position,
                           name);
                    }
                    // There's no quick and easy way to do this,
                    // but most resource streams are initialized using
                    // byte*'s anyway.
                    catch (NotSupportedException)
                    {
                        goto loadNormally;
                    }
                }
            }
            loadNormally:
                var ms = new MemoryStream();
                data.CopyTo(ms);
                var arr = ms.ToArray();
                ms.Dispose();
                return Load(arr, name);
        }

        private static unsafe NativeAssembly LoadInternal(byte* data, long length, string name = null)
        {
            lock (_handles)
            {
                var asm = new NativeAssembly();
                IntPtr handle = NativeAssemblyImpl.LoadLibrary(
                    data,
                    length,
                    loadLibrary: asm.LoadLibraryDelegate,
                    getProcAddress: GetProcAddressDelegate,
                    freeLibrary: FreeLibraryDelegate
                );
                _handles.Add(handle, new LibraryInfo(name));
                if (!string.IsNullOrEmpty(name))
                {
                    if (!_libraryMap.ContainsKey(name))
                    {
                        _libraryMap.Add(name, new HashSet<IntPtr>());
                    }
                    _libraryMap[name].Add(handle);
                }

                asm._name = name;
                asm._handle = handle;

                return asm;
            }
        }

        /// <summary>
        /// Unloads the library. You do not need to
        /// call this function, <code>Dispose()</code>
        /// does the work for you.
        /// </summary>
        public void Unload()
        {
            IntPtr deadPtr = default;
            Interlocked.Exchange(ref _handle, deadPtr);

            FreeLibraryHandle(deadPtr);
        }

        /// <summary>
        /// Gets a function from the loaded assembly.
        /// </summary>
        /// <typeparam name="T">A compatible delegate type.</typeparam>
        /// <param name="name">The exported function's name</param>
        /// <returns>A delegate to the exported function.</returns>
        public T GetDelegate<T>(string name) where T : Delegate
        {
            return NativeAssemblyImpl.GetDelegate<T>(_handle, name);
        }

        public static event NativeResolveEventHandler AssemblyResolve;

        protected static bool FreeLibraryHandle(IntPtr handle)
        {
            lock (_handles)
            {
                try
                {
                    if (handle != IntPtr.Zero)
                    {
                        var info = _handles[handle];

                        NativeAssemblyImpl.FreeLibrary(handle);
                        _handles.Remove(handle);

                        --info.RefCount;

                        if (info.RefCount == 0)
                        {
                            var nameSet = _libraryMap[info.Name];
                            nameSet.Remove(handle);

                            if (nameSet.Count == 0)
                            {
                                _libraryMap.Remove(info.Name);
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"Unhandled exception: {e} while " +
                        $"trying to free library handle ${handle}");
                    return false;
                }
            }
        }

        protected static IntPtr ResolveLibraryHandle(string name, NativeAssembly reqestingAssembly)
        {
            lock (_handles)
            {
                if (_libraryMap.TryGetValue(name, out var librarySet))
                {
                    var handle = librarySet.First();
                    var info = _handles[handle];
                    ++info.RefCount;

                    return handle;
                }
                else
                {
                    var list = AssemblyResolve?.GetInvocationList();

                    if (list != null)
                    {
                        foreach (NativeResolveEventHandler assemblyResolver in list)
                        {
                            var currentArgs = new NativeResolveEventArgs(name, reqestingAssembly);
                            var asm = assemblyResolver(null, currentArgs);

                            if (asm._disposedValue)
                            {
                                Debug.WriteLine("[Warning]: Disposed assembly provided.");
                            }

                            if (asm != null && asm._handle != null)
                            {
                                var handle = asm._handle;
                                ++_handles[handle].RefCount;
                                if (currentArgs.ShouldDisposeAssembly)
                                {
                                    asm.Dispose();
                                }
                                return handle;
                            }
                        }
                    }

                    return IntPtr.Zero;
                }
            }
        }

        private unsafe void* LoadLibraryUnsafe(char* filename, void* userdata)
        {
            void* handle = NativeAssemblyImpl.MemoryDefaultLoadLibrary(filename, userdata);

            if (handle == null)
            {
                return (void*)ResolveLibraryHandle(
                    Marshal.PtrToStringAnsi((IntPtr)filename),
                    this);
            }

            return handle;
        }

        private static unsafe void* GetProcAddressUnsafe(void* module, void* name, void* userdata)
        {
            IntPtr handle = (IntPtr)module;
            lock (_handles)
            {
                if (_handles.ContainsKey(handle))
                {
                    return NativeAssemblyImpl.GetProcAddress(module, (char *)name);
                }
            }

            return NativeAssemblyImpl.MemoryDefaultGetProcAddress(module, name, userdata);
        }

        private static unsafe bool FreeLibraryUnsafe(void* module, void* userdata)
        {
            FreeLibraryHandle((IntPtr)module);

            return NativeAssemblyImpl.MemoryDefaultFreeLibrary(module, userdata);
        }

        #region Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Unload();
                _disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~NativeAssembly()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Disposes the library, unloading it from the process.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
