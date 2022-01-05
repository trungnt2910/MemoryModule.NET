using MemoryModule.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    class MachoInitializer : IInitializer
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void InitDelegate(int argc, byte** argv, byte** envp, byte** apple);

        private IntPtr _addr;
        private InitDelegate _del;

        public Type[] Arguments { get; } = new Type[] { typeof(int), typeof(IntPtr), typeof(IntPtr), typeof(IntPtr) };

        private static readonly string[] _argvString;
        private static readonly string[] _envpString;
        private static readonly string[] _appleString;

        static MachoInitializer()
        {
            _argvString = Environment.GetCommandLineArgs();
            _envpString = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Select(x => $"{x.Key}={x.Value}")
                .ToArray();
            _appleString = new string[] { Assembly.GetEntryAssembly().Location };
        }

        public bool Run()
        {
            return RunInternal(_argvString.Length, _argvString, _envpString, _appleString);
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

            return RunInternal((int)args[0], (string[])args[1], (string[])args[2], (string[])args[3]);
        }

        private unsafe bool RunInternal(int argc, string[] argv, string[] envp, string[] apple)
        {
            _del = Marshal.GetDelegateForFunctionPointer<InitDelegate>(_addr);

            var argvArr = argv.Select(str => Marshal.StringToHGlobalAnsi(str)).ToArray();
            var envpArr = envp.Select(str => Marshal.StringToHGlobalAnsi(str)).Concat(new[] { IntPtr.Zero }).ToArray();
            var appleArr = apple.Select(str => Marshal.StringToHGlobalAnsi(str)).Concat(new[] { IntPtr.Zero }).ToArray();

            fixed (IntPtr* argvPtr = &argvArr[0])
            fixed (IntPtr* envpPtr = &envpArr[0])
            fixed (IntPtr* applePtr = &appleArr[0])
            {
                _del(argc, (byte**)argvPtr, (byte**)envpPtr, (byte**)applePtr);
            }

            foreach (var ptr in argvArr.Concat(envpArr.Reverse().Skip(1)).Concat(appleArr.Reverse().Skip(1)))
            {
                Marshal.FreeHGlobal(ptr);
            }

            return true;
        }

        internal MachoInitializer(IntPtr del)
        {
            _addr = del;
        }
    }
}
