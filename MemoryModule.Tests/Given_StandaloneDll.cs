using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MemoryModule.Tests
{
    [TestClass]
    public class Given_StandaloneDll
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int GetSecretDelegate();

        [TestMethod]
        public void When_Loaded()
        {
            Helper.PrintEnvironmentDetails();

            // Sometimes, low-level segmentation fault can occur, without crashing the test.
            for (int test = 0; test < 10; ++test)
            {
                string dllName = Helper.GetDllName("Secret");
                var dllStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"{Helper.ModuleName}.{dllName}");
                var asm = NativeAssembly.Load(dllStream);

                var secretFunc = asm.GetDelegate<GetSecretDelegate>("GetSecret");

                for (int i = 0; i < 10; ++i)
                {
                    int result = secretFunc();
                    Assert.AreEqual(result, Math.Min(Math.Max(0, result), 100));
                }

                asm.Dispose();
            }
        }
    }
}