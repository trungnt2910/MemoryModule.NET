using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MemoryModule.Tests
{
    [TestClass]
    public class Given_MemoryModulePP
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int LdrLoadDllMemoryExWDelegate(
            [Out] out IntPtr BaseAddress,
            [Out] out IntPtr LdrEntry,
            [In] uint dwFlags,
            [In][MarshalAs(UnmanagedType.LPArray)] byte[] BufferAddress,
            [In] UIntPtr BufferSize,
            [In][MarshalAs(UnmanagedType.LPWStr)] string DllName,
            [In][MarshalAs(UnmanagedType.LPWStr)] string DllFullName
        );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool LdrUnloadDllMemoryDelegate([In] IntPtr BaseAddress);

        const uint LOAD_FLAGS_PASS_IMAGE_CHECK = 0x40000000;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int GetSecretDelegate();

        private readonly LdrLoadDllMemoryExWDelegate LdrLoadDllMemoryExW;
        private readonly LdrUnloadDllMemoryDelegate LdrUnloadDllMemory;
        private readonly bool __skip = false;
        private readonly NativeAssembly __mmppAsm;
        private readonly IntPtr __secretHandle;

        public Given_MemoryModulePP()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var mmppDll = Helper.GetDllName("MemoryModulePP");
                using var mmppDllStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"{Helper.ModuleName}.{mmppDll}");
                __mmppAsm = NativeAssembly.Load(mmppDllStream, mmppDll);

                LdrLoadDllMemoryExW =
                    __mmppAsm.GetDelegate<LdrLoadDllMemoryExWDelegate>("LdrLoadDllMemoryExW");
                LdrUnloadDllMemory =
                    __mmppAsm.GetDelegate<LdrUnloadDllMemoryDelegate>("LdrUnloadDllMemory");

                var secretDll = Helper.GetDllName("Secret");
                using var secretDllStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"{Helper.ModuleName}.{secretDll}");
                using var secretDllMs = new MemoryStream();
                secretDllStream.CopyTo(secretDllMs);
                var secretDllBytes = secretDllMs.ToArray();

                LdrLoadDllMemoryExW(out __secretHandle, out _,
                    LOAD_FLAGS_PASS_IMAGE_CHECK, secretDllBytes, 0, "secret", null);
            }
            else
            {
                __skip = true;
            }
        }

        [TestMethod]
        public void When_UsedWithDotNetNativeLibrary()
        {
            Helper.PrintEnvironmentDetails();

            if (__skip)
            {
                return;
            }

            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(),
                (name, asm, path) =>
                {
                    if (name == "secret")
                    {
                        return __secretHandle;
                    }

                    return IntPtr.Zero;
                });

            for (int i = 0; i < 10; ++i)
            {
                [DllImport("secret")]
                static extern int GetSecret();

                int result = GetSecret();
                Assert.AreEqual(result, Math.Min(Math.Max(0, result), 100));
            }
        }

        [TestMethod]
        public void When_UsedWithGetProcAddress()
        {
            Helper.PrintEnvironmentDetails();

            if (__skip)
            {
                return;
            }

            {
                [DllImport("kernel32.dll", SetLastError = true)]
                static extern IntPtr GetProcAddress(
                    IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

                var secretProc = GetProcAddress(__secretHandle, "GetSecret");
                var GetSecret =
                    Marshal.GetDelegateForFunctionPointer<GetSecretDelegate>(secretProc);

                for (int i = 0; i < 10; ++i)
                {
                    int result = GetSecret();
                    Assert.AreEqual(result, Math.Min(Math.Max(0, result), 100));
                }
            }
        }
    }
}
