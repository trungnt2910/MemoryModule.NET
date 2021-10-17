using MemoryModule;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace DemoApp
{
    static class Program
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int addNumberProc(int a, int b);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int GetIntDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GreetProc();

        static int Main(string[] args)
        {
            var asmBytes = File.ReadAllBytes($"SampleDLL{(Environment.Is64BitProcess ? "64" : "")}.dll");

            NativeAssembly.AssemblyResolve += NativeAssembly_AssemblyResolve;

            var asm = NativeAssembly.Load(asmBytes);

            Console.WriteLine("Successfully loaded library.");
            var addNumberFunc = asm.GetDelegate<addNumberProc>("addNumbers");
            var rand = new Random();
            int num1 = rand.Next(0, 20);
            int num2 = rand.Next(0, 20);
            Console.WriteLine($"{num1}+{num2}={addNumberFunc(num1, num2)}");

            var getThreadLocalInt = asm.GetDelegate<GetIntDelegate>("GetThreadLocalInt");

            for (int i = 0; i < 3; ++i)
            {
                Console.WriteLine($"Current global int: {getThreadLocalInt()}");
            }

            var thread = new Thread(() =>
            {
                Console.WriteLine("On second thread...");
                for (int i = 0; i < 3; ++i)
                {
                    Console.WriteLine($"Current global int: {getThreadLocalInt()}");
                }
            });

            thread.Start();
            thread.Join();

            var greetFunc = asm.GetDelegate<GreetProc>("Greet");
            greetFunc();

            asm.Dispose();
            return 0;
        }

        private static NativeAssembly NativeAssembly_AssemblyResolve(object sender, NativeResolveEventArgs e)
        {
            Console.WriteLine(e.RequestingAssembly);
            Console.WriteLine(e.Name);

            string secretAsmName = $"Secret{(Environment.Is64BitProcess ? "64" : "")}.dll";
            if (e.Name.Equals(secretAsmName, StringComparison.InvariantCultureIgnoreCase))
            {
                using (var resourceStream =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream($"DemoApp.{secretAsmName}"))
                {
                    var asm = NativeAssembly.Load(resourceStream, secretAsmName);
                    // We are returning a newly-loaded assembly,
                    // so the resolver should dispose this library,
                    // and decrement the reference count.
                    e.ShouldDisposeAssembly = true;
                    return asm;
                }
            }

            return null;
        }
    }
}
