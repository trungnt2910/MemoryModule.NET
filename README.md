# MemoryModule.NET

![BuildStatus](https://github.com/trungnt2910/MemoryModule.NET/actions/workflows/ci.yml/badge.svg)
| Package | Version |
| ---- | ---- |
| Cross platform | [![CrossPlatformShield](https://shields.io/nuget/vpre/MemoryModule)](https://www.nuget.org/packages/MemoryModule) |
| Windows | [![WindowsShield](https://shields.io/nuget/vpre/MemoryModule.Compact.Windows)](https://www.nuget.org/packages/MemoryModule.Compact.Windows) |
| Linux | Coming soon |
| MacOS | Coming soon |

Loads unmanaged libraries right from your embedded resources!
Works on Windows and Linux only, both on .NET Framework and .NET Core (and of course .NET 5.0)

## Features:
- Load x86 and x64 assemblies, right from the memory. No P/Invoke, no temporary files.
- Correctly resolves loaded unmanaged libraries, and exposes a `AssemblyResolve` event for users to manually handle it.
- Linux x86 (32 and 64 bit) support, through interoperation with `glibc`.

## Sample code:
```C#
using MemoryModule;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int addNumberProc(int a, int b);

            var asmBytes = File.ReadAllBytes($"SampleDLL.dll");

            var asm = NativeAssembly.Load(asmBytes);

            Console.WriteLine("Successfully loaded library.");
            var addNumberFnc = asm.GetDelegate<addNumberProc>("addNumbers");
            var rand = new Random();
            int num1 = rand.Next(0, 20);
            int num2 = rand.Next(0, 20);
            Console.WriteLine($"{num1}+{num2}={addNumberFunc(num1, num2)}");

            var greetFunc = asm.GetDelegate<GreetProc>("Greet");
            greetFunc();

            asm.Dispose();
```


See the DemoApp for more details.

## Packages
- [MemoryModule](https://www.nuget.org/packages/MemoryModule): Cross platform package, with support for x86 and x86_64, Windows and Linux.
- [MemoryModule.Compact.Windows](https://www.nuget.org/packages/MemoryModule.Compact.Windows): Compact module for small, standalone Windows applications.

## Known issues
- Windows: Beware of ~~64-bit~~ `dll` files compiled using g++: https://github.com/fancycode/MemoryModule/issues/108. These files must be compiled using `-static-libgcc` and `-static-libstdc++` to load properly, in both the original C version and this version.
- Windows: Resources are not supported.
- Linux: Support is limited. While basic C/C++ libraries, such as `libcurl`, can be properly loaded, MemoryModule.NET may not work with other advanced libraries that contain unknown ELF relocations. If that's the case, please [open an issue](https://github.com/trungnt2910/MemoryModule.NET/issues).
- Linux: As MemoryModule.NET relies on certain `glibc` data structures, it may fail on systems that use beta/custom `glibc` version. Please [open an issue](https://github.com/trungnt2910/MemoryModule.NET/issues) for support.