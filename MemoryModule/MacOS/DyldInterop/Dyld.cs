using MemoryModule;
using MemoryModule.MacOS.Macho;
using MemoryModule.MacOS.Macho.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DyldInterop
{
    unsafe static class Dyld
    {
        const string libSystem = "/usr/lib/libSystem.dylib";

        // The tlv family is contained in libdyld, not dyld.
        const string dyld = "/usr/lib/dyld";

        [DllImport(libSystem)]
        private static extern int mach_vm_region_recurse(uint target_task, ref UIntPtr address, ref UIntPtr size, ref uint nesting_depth, [MarshalAs(UnmanagedType.LPArray)] int[] info, ref uint infoCnt);

        [DllImport(libSystem)]
        private static extern void pid_for_task(uint task, out int pid);

        [DllImport(libSystem, SetLastError = true)]
        private static extern int proc_regionfilename(int pid, ulong address, IntPtr buffer, uint buffersize);

        [DllImport(libSystem)]
        private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string name, int mode = 1);

        [DllImport(libSystem)]
        private static extern IntPtr dlsym(IntPtr handle, char* name);

        [DllImport(libSystem)]
        private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport(libSystem)]
        private static extern int dlclose(IntPtr handle);

        private static IntPtr _libSystemHandle = dlopen(libSystem);
        private static uint* _mach_task_self = (uint*)dlsym(_libSystemHandle, "mach_task_self_");

        private static uint mach_task_self() => *_mach_task_self;

        public class MemoryRegion
        {
            public string File { get; set; }
            public IntPtr Location { get; set; }
        }

        public static IEnumerable<MemoryRegion> GetRegions()
        {
            UIntPtr start = UIntPtr.Zero;
            pid_for_task(mach_task_self(), out var pid);

            while (true)
            {
                UIntPtr address = start;
                UIntPtr size = UIntPtr.Zero;
                uint depth = uint.MaxValue;
                uint count = 16;
                int[] sink = new int[count];

                int result = mach_vm_region_recurse(mach_task_self(), ref address, ref size, ref depth, sink, ref count);

                if (result != 0)
                {
                    yield break;
                }

                string name = null;
                try
                {
                    const int bufferLength = 4096;
                    var buffer = Marshal.AllocHGlobal(bufferLength);
                    result = proc_regionfilename(pid, (ulong)address, buffer, (uint)bufferLength);

                    if (result <= 0)
                    {
                        name = null;
                    }
                    else
                    {
                        name = Marshal.PtrToStringAnsi(buffer);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                //Console.WriteLine($"Address: 0x{(ulong)address:x}, Name: {name}.");
                yield return new MemoryRegion()
                {
                    File = name,
                    Location = (IntPtr)(ulong)address
                };

                start = (UIntPtr)((ulong)address + (ulong)size);
            }
        }

        // dyld should be present, before the CoreCLR even loads.
        //public readonly static IntPtr Address = GetRegions()
        //    .Where(loc => loc.File == dyld)
        //    .OrderBy(loc => (ulong)loc.Location)
        //    .First().Location;

        //private static MachoHeader _header = new MachoHeader((byte*)Address, (byte*)Address);
        //private static Dictionary<string, IntPtr> _map = _header.BuildRuntimeSymbolTable();

        private static Dictionary<string, IntPtr> _map = LocateMap();

        private static Dictionary<string, IntPtr> LocateMap()
        {
            var regions = GetRegions();

            foreach (var loc in regions)
            {
                if (loc.File != dyld)
                {
                    continue;
                }

                var addr = (byte*)loc.Location;

                try
                {
                    var header = new MachoHeader(addr, addr);
                    return header.BuildRuntimeSymbolTable();
                }
                catch
                {
                    continue;
                }
            }

            throw new NativeAssemblyLoadException("Cannot find dyld.");
        }

        private static TFunc Load<TFunc>(string name) where TFunc : Delegate
        {
            return Marshal.GetDelegateForFunctionPointer<TFunc>(_map[$"_{name}"]);
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate uint _dyld_image_count_proc();
        static _dyld_image_count_proc _dyld_image_count = Load<_dyld_image_count_proc>(nameof(_dyld_image_count));

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate MachoHeaderNative* _dyld_get_image_header_proc(uint imageIndex);
        static _dyld_get_image_header_proc _dyld_get_image_header = Load<_dyld_get_image_header_proc>(nameof(_dyld_get_image_header));

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        // Can't use MarshalAs, dotnet will try to free stuff.
        delegate IntPtr _dyld_get_image_name_proc(uint imageIndex);
        static _dyld_get_image_name_proc _dyld_get_image_name = Load<_dyld_get_image_name_proc>(nameof(_dyld_get_image_name));

        public static uint ImageCount => _dyld_image_count();

        public static string GetImageName(uint imageIndex)
        {
            return Marshal.PtrToStringAnsi(_dyld_get_image_name(imageIndex));
        }

        public static MachoHeader GetHeader(uint imageIndex)
        {
            var ptr = _dyld_get_image_header(imageIndex);
            return new MachoHeader((byte*)ptr, (byte*)ptr);
        }

        public static IEnumerable<(string name, MachoHeader header)> GetLibraries()
        {
            var count = ImageCount;
            for (uint i = 0; i < count; ++i)
            {
                yield return (GetImageName(i), GetHeader(i));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct NSObjectFileImage
        {
            public IntPtr image;
            public byte* imageBaseAddress;   // not used with OFI created from files
            public UIntPtr imageLength;		  // not used with OFI created from files
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate IntPtr _dyld_loadFromMemoryProc(byte* mem, ulong len, [MarshalAs(UnmanagedType.LPStr)] string moduleName);
        static _dyld_loadFromMemoryProc loadFromMemory = Load<_dyld_loadFromMemoryProc>("_ZN4dyld14loadFromMemoryEPKhyPKc"); //dyld::loadFromMemory

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate IntPtr _dyld_NSLinkModuleProc(ref NSObjectFileImage objectFileImage, [MarshalAs(UnmanagedType.LPStr)] string moduleName, uint options);
        static _dyld_NSLinkModuleProc NSLinkModule = Load<_dyld_NSLinkModuleProc>("NSLinkModule");

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        delegate IntPtr _dyld_NSUnLinkModuleProc(ref IntPtr handle, uint options);
        static _dyld_NSUnLinkModuleProc NSUnLinkModule = Load<_dyld_NSUnLinkModuleProc>("NSUnLinkModule");

        const uint NSLINKMODULE_OPTION_CAN_UNLOAD = 0x20;

        private static Dictionary<IntPtr, int> _refCount = new Dictionary<IntPtr, int>();

        public static IntPtr Load(byte* buffer, ulong length, string name)
        {
            var result = loadFromMemory(buffer, length, name);

            if (result == IntPtr.Zero)
            {
                throw new NativeAssemblyLoadException("Failed to load from memory.");
            }

            var objectFileImage = new NSObjectFileImage()
            {
                image = result,
                imageBaseAddress = buffer,
                imageLength = (UIntPtr)length
            };

            // To increment the refcount.
            NSLinkModule(ref objectFileImage, name, NSLINKMODULE_OPTION_CAN_UNLOAD);

            return result;
        }

        public static bool Unload(IntPtr handle)
        {
            return dlclose(handle) == 0;
        }

        public static IntPtr Sym(IntPtr handle, char* name)
        {
            return dlsym(handle, name);
        }
    }
}

