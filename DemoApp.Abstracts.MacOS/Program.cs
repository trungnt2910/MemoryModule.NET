using MemoryModule.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DemoApp.Abstracts.MacOS
{
    unsafe class Program
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int GetScretProc();

        static void Main(string[] args)
        {
            var asm = Assembly.GetExecutingAssembly().GetManifestResourceStream($"DemoApp.Abstracts.MacOS.Secret{(Environment.Is64BitProcess ? "64" : "")}.dylib") as UnmanagedMemoryStream;

            var module = Loader.Load((IntPtr)asm.PositionPointer);

            Loader.AllocateSections(module, NativeFunctions.Default);
            Loader.PerformRebase(module);
            Loader.PerformBinding(module);
            Loader.PerformPageProtection(module);
            Loader.PerformInitialization(module);

            var getSecretPtr = module.Exports.FirstOrDefault(sym => sym.Name == "GetSecret")?.Address ?? IntPtr.Zero;
            var getSecret = Marshal.GetDelegateForFunctionPointer<GetScretProc>(getSecretPtr);

            Console.WriteLine(getSecret());

            Loader.PerformFinalization(module);
            Loader.UnloadReferences(module);
            Loader.DeallocateSections(module);
        }
    }
}
