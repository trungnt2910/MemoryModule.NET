using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Tests
{
    internal static class Helper
    {
        public static string GetDllName(string displayName)
        {
            return $"{displayName}{(Environment.Is64BitProcess ? "64" : "")}.dll";
        }
    }
}
