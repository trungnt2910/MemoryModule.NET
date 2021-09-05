using NUnit.Framework;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MemoryModule.Tests
{
    public class Given_StandaloneDll
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int GetSecretDelegate();

        [Test]
        public void When_Loaded()
        {
            Helper.PrintEnvironmentDetails();

            // Sometimes, low-level segmentation fault can occur, without crashing the test.
            for (int test = 0; test < 10; ++test)
            {
                string dllName = Helper.GetDllName("Secret");
                var dllStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"MemoryModule.Tests.{dllName}");
                using var asm = NativeAssembly.Load(dllStream);

                var secretFunc = asm.GetDelegate<GetSecretDelegate>("GetSecret");

                for (int i = 0; i < 10; ++i)
                {
                    int result = secretFunc();
                    Assert.AreEqual(result, Math.Clamp(result, 0, 100));
                }
            }
        }
    }
}