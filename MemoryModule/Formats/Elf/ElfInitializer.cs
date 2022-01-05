using MemoryModule.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Elf
{
    class ElfInitializer : IInitializer
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void InitDelegate(int argc, byte** argv, byte** envp);

        private InitDelegate _del;

        public Type[] Arguments { get; } = new Type[] { typeof(int) /*argc*/, typeof(string[]) /*argv*/, typeof(string[]) /*envp*/ };

        public IntPtr Address { get; internal set; }

        private static readonly string[] _argvString;
        private static readonly string[] _envpString;

        static ElfInitializer()
        {
            _argvString = Environment.GetCommandLineArgs();
            _envpString = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Select(x => $"{x.Key}={x.Value}")
                .ToArray();
        }

        public bool Run()
        {
            return RunInternal(_argvString.Length, _argvString, _envpString);
        }

        public bool Run(params object[] args)
        {
            if (args.Length != Arguments.Length)
            {
                return false;
            }

            for (int i = 0; i < args.Length; ++i)
            {
                if (!Arguments[i].IsAssignableFrom(args[i].GetType()))
                {
                    return false;
                }
            }

            return RunInternal((int)args[0], (string[])args[1], (string[])args[2]);
        }

        private unsafe bool RunInternal(int argc, string[] argv, string[] envp)
        {
            _del = Marshal.GetDelegateForFunctionPointer<InitDelegate>(Address);

            var argvArr = argv.Select(str => Marshal.StringToHGlobalAnsi(str)).ToArray();
            var envpArr = argv.Select(str => Marshal.StringToHGlobalAnsi(str)).Concat(new[] { IntPtr.Zero }).ToArray();

            fixed (IntPtr* argvPtr = &argvArr[0])
            fixed (IntPtr* envpPtr = &envpArr[0])
            {
                _del(argc, (byte**)argvPtr, (byte**)envpPtr);
            }

            foreach (var ptr in argvArr.Concat(envpArr.Reverse().Skip(1)))
            {
                Marshal.FreeHGlobal(ptr);
            }

            return true;
        }

        internal ElfInitializer(IntPtr del)
        {
            Address = del;
        }
    }
}
