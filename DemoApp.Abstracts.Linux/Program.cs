using MemoryModule.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace DemoApp.Abstracts.Linux
{
    unsafe class Program
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int GetScretProc();

        static void Main(string[] args)
        {
            var asm = Assembly.GetExecutingAssembly().GetManifestResourceStream($"DemoApp.Abstracts.Linux.Tls{(Environment.Is64BitProcess ? "64" : "")}.so") as UnmanagedMemoryStream;

            var module = Loader.Load((IntPtr)asm.PositionPointer);

            Loader.AllocateSections(module, NativeFunctions.Default);
            Loader.PerformRebase(module);
            Loader.PerformBinding(module);
            Loader.PerformPageProtection(module);
            Loader.PerformInitialization(module);

            var getSecretPtr = module.Exports.FirstOrDefault(sym => sym.Name == "GetThreadLocalInt")?.Address ?? IntPtr.Zero;
            var getSecret = Marshal.GetDelegateForFunctionPointer<GetScretProc>(getSecretPtr);

            Console.WriteLine("Done.");
            for (int i = 0; i < 10; ++i)
            {
                Console.WriteLine(getSecret());
            }

            var thread = new Thread(() =>
            {
                for (int i = 0; i < 10; ++i)
                {
                    Console.WriteLine(getSecret());
                }
            });

            thread.Start();
            thread.Join();

            Loader.PerformFinalization(module);
            Loader.UnloadReferences(module);
            Loader.DeallocateSections(module);
        }
    }
}
