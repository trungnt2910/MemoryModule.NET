using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using MemoryModule;

namespace DemoApp
{
    static class Program
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int addNumberProc(int a, int b);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void GreetProc();

        [DllImport("SampleDLL64.dll", EntryPoint = "Greet", CallingConvention = CallingConvention.Cdecl)]
        static extern void Greet64();

        static int Main(string[] args)
        {
            if (Environment.Is64BitProcess)
            {
                Greet64();
            }

            var asmBytes = File.ReadAllBytes($"SampleDLL{(Environment.Is64BitProcess ? "64" : "")}.dll");

            var asm = NativeAssembly.Load(asmBytes);

            Console.WriteLine("Successfully loaded library.");
            var addNumberFunc = asm.GetDelegate<addNumberProc>("addNumbers");
            var rand = new Random();
            int num1 = rand.Next(0, 20);
            int num2 = rand.Next(0, 20);
            Console.WriteLine($"{num1}+{num2}={addNumberFunc(num1, num2)}");

            var greetFunc = asm.GetDelegate<GreetProc>("Greet");
            greetFunc();

            asm.Dispose();
            return 0;
        }
    }
}
