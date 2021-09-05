using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Tests
{
    internal static class Helper
    {
        public static string GetDllName(string displayName)
        {
            return $"{displayName}{(Environment.Is64BitProcess ? "64" : "")}.{GetDllExtension()}";
        }

        public static string GetDllExtension()
        {
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
        }

    }
}
