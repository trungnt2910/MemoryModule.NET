using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GlibcInterop
{
    internal static class GlibcEnvironment
    {
        [DllImport("libc")]
        private static extern IntPtr gnu_get_libc_version();

        [DllImport("libc")]
        private static extern void _dl_get_tls_static_info(out UIntPtr size, out UIntPtr align);

        public static readonly Version Version;
        public static readonly Version FirstAvailableVersion = new Version(2, 21);
        public static readonly Version LatestAvailableVersion = new Version(2, 34);

        static GlibcEnvironment()
        {
            try
            {
                // We cannot use the MarshalAsAttribute here, as 
                // glibc returns a string in read only memory, while
                // .NET tries to free that memory after marshalling.
                var libcVerString = Marshal.PtrToStringAnsi(gnu_get_libc_version());
                var libcVer = Version.Parse(libcVerString);

                Version = libcVer;

                if (Version < FirstAvailableVersion)
                {
                    var message = $"Your glibc version, {Version} is too old!\n" +
                        $"Please open an issue at github.com/trungnt2910/MemoryModule.NET " +
                        $"to remove this message.";

                    Version = FirstAvailableVersion;

                    Console.WriteLine(message);
                    Debug.WriteLine(message);
                }

                if (Version > LatestAvailableVersion)
                {
                    var message = $"Are you using a beta glibc build?\n" +
                        "If you're not, please update this package!\n" +
                        "If this message continues to show up, " +
                        $"please open an issue at github.com/trungnt2910/MemoryModule.NET " +
                        $"to remove it.";

                    Version = LatestAvailableVersion;

                    Console.WriteLine(message);
                    Debug.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                var message = $"Error when trying to get glibc version: {e}\n" +
                    $"Please open an issue at github.com/trungnt2910/MemoryModule.NET";

                Version = LatestAvailableVersion;

                Console.WriteLine(message);
                Debug.WriteLine(message);
            }
        }
    }
}