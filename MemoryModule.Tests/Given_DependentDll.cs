using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MemoryModule.Tests
{
    [TestClass]
    public class Given_DependentDll
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int AddNumberDelegate(int a, int b);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GreetDelegate();

        [TestMethod]
        public void When_DependencyPreLoaded()
        {
            Helper.PrintEnvironmentDetails();

            var random = new Random();

            for (int test = 0; test < 10; ++test)
            {
                string secretDll = Helper.GetDllName("Secret");
                var secretDllStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"MemoryModule.Tests.{secretDll}");
                using var secretAsm = NativeAssembly.Load(secretDllStream, secretDll);

                // That's why I hate Linux. Case sensitive file systems.
                string dllName = Helper.GetDllName("SampleDll");
                var dllStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"MemoryModule.Tests.{dllName}");
                using var asm = NativeAssembly.Load(dllStream);

                var addNumbers = asm.GetDelegate<AddNumberDelegate>("addNumbers");

                for (int i = 0; i < 100; ++i)
                {
                    const int low = -1000000000;
                    const int high = -low;

                    var a = random.Next(low, high);
                    var b = random.Next(low, high);

                    Assert.AreEqual(a + b, addNumbers(a, b));
                }

                // Test if C++ causes any problems...
                var Greet = asm.GetDelegate<GreetDelegate>("Greet");
                Greet();
            }
        }

        [TestMethod]
        public void When_DependencyLateResolve()
        {
            Helper.PrintEnvironmentDetails();

            var random = new Random();

            NativeAssembly.AssemblyResolve += NativeAssembly_AssemblyResolve;

            for (int test = 0; test < 10; ++test)
            {
                string dllName = Helper.GetDllName("SampleDll");
                var dllStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"MemoryModule.Tests.{dllName}");
                using var asm = NativeAssembly.Load(dllStream);

                var addNumbers = asm.GetDelegate<AddNumberDelegate>("addNumbers");

                for (int i = 0; i < 100; ++i)
                {
                    const int low = -1000000000;
                    const int high = -low;

                    var a = random.Next(low, high);
                    var b = random.Next(low, high);

                    Assert.AreEqual(a + b, addNumbers(a, b));
                }

                // Test if C++ causes any problems...
                var Greet = asm.GetDelegate<GreetDelegate>("Greet");
                Greet();
            }

            NativeAssembly NativeAssembly_AssemblyResolve(object sender, NativeResolveEventArgs args)
            {
                if (args.Name.Equals(Helper.GetDllName("Secret")))
                {
                    args.ShouldDisposeAssembly = true;
                    return NativeAssembly.Load(
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(
                            $"MemoryModule.Tests.{Helper.GetDllName("Secret")}"
                            )
                        );
                }

                return null;
            }
        }
    }
}
