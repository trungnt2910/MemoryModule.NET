using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Tests
{
    internal static class Helper
    {
#if WINDOWS
        public const string ModuleName = "MemoryModule.Compact.Windows.Tests";
#else
        public const string ModuleName = "MemoryModule.Tests";
#endif

        public static string GetDllName(string displayName)
        {
            return $"{displayName}{(Environment.Is64BitProcess ? "64" : "")}.{GetDllExtension()}";
        }

        public static string GetDllExtension()
        {
#if WINDOWS
            return "dll";
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "so";
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported platform.");
            }
#endif
        }

        public static void PrintEnvironmentDetails()
        {
#if WINDOWS
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine($"glibc version: {GlibcInterop.GlibcEnvironment.Version}");
            }
#endif
        }
    }
}
