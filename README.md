# MemoryModule.NET

Loads unmanaged libraries right from your embedded resources!
Works on Windows only, both on .NET Framework and .NET Core (and of course .NET 5.0)

## Sample code:
```C#
using MemoryModule;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int addNumberProc(int a, int b);

            var asmBytes = File.ReadAllBytes($"SampleDLL.dll");

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
```


See the DemoApp for more details.

## Known issues
- Beware of 64-bit `dll` files compiled using g++: https://github.com/fancycode/MemoryModule/issues/108.