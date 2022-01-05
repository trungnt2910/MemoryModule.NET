using Iced.Intel;
using MemoryModule.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static Iced.Intel.AssemblerRegisters;

namespace MemoryModule.Tls.x86_64
{
    unsafe static class HookGenerator
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void GetCpuInfoDelegate();

        [StructLayout(LayoutKind.Sequential)]
        private struct CpuInfo
        {
            public bool HasXSave;
            public uint XSaveBufferSize;
            public uint XSaveLowFlags;
            public uint XSaveHighFlags;
        }

        private static readonly CpuInfo _cpuInfo = GetCpuInfo();
        private static readonly bool _isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
        private static readonly bool _isUnix = Environment.OSVersion.Platform == PlatformID.Unix;

        public static byte[] GenerateHookFunction(AssemblerRegister64 hookSource, AssemblerRegister64 hookDest, TlsHookFunctionDelegate del)
        {
            AssemblerRegister64 managedSource, managedDest;
            if (_isWindows)
            {
                managedSource = rcx;
                managedDest = rax;
            }
            else if (_isUnix)
            {
                managedSource = rdi;
                managedDest = rax;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            var asm = new Assembler(64);
            asm.push(rbp);
            asm.mov(rbp, rsp);
            asm.sub(rsp, 0x80);

            asm.mov(__qword_ptr[rbp - 0x8], rdi);
            asm.mov(__qword_ptr[rbp - 0x10], rsi);
            asm.mov(__qword_ptr[rbp - 0x18], rax);
            asm.mov(__qword_ptr[rbp - 0x20], rbx);
            asm.mov(__qword_ptr[rbp - 0x28], rcx);
            asm.mov(__qword_ptr[rbp - 0x30], rdx);
            asm.mov(__qword_ptr[rbp - 0x38], r8);
            asm.mov(__qword_ptr[rbp - 0x40], r9);
            asm.mov(__qword_ptr[rbp - 0x48], r10);
            asm.mov(__qword_ptr[rbp - 0x50], r11);
            asm.mov(__qword_ptr[rbp - 0x58], r12);
            asm.mov(__qword_ptr[rbp - 0x60], r13);
            asm.mov(__qword_ptr[rbp - 0x68], r14);
            asm.mov(__qword_ptr[rbp - 0x70], r15);
            asm.mov(__qword_ptr[rbp - 0x78], hookSource);

            // Here, we take advantage of JIT compilation:
            // We can generate different assembly depending on
            // whether the CPU supports XSAVE or not, without
            // having to branch every time.
            if (_cpuInfo.HasXSave)
            {
                asm.mov(rdi, rsp);
                asm.sub(rdi, (int)_cpuInfo.XSaveBufferSize);
                // Align stack to 64 bytes.
                asm.and(rdi, -64);

                asm.mov(rsp, rdi);

                asm.xor(rbx, rbx);
                asm.mov(r9, rsp);
                asm.mov(r10, rsp);
                asm.add(r10, (int)_cpuInfo.XSaveBufferSize);

                // Memory needs to be zeroed out.
                var temp = asm.CreateLabel("fillMem");
                asm.Label(ref temp);
                asm.mov(__byte_ptr[r9], rbx);
                asm.add(r9, 0x8);
                asm.cmp(r9, r10);
                asm.jne(temp);

                asm.mov(eax, (int)_cpuInfo.XSaveLowFlags);
                asm.mov(edx, (int)_cpuInfo.XSaveHighFlags);
                asm.xsave(__[rsp]);
            }
            else
            {
                asm.sub(rsp, 0x80);
                asm.movdqa(__xmmword_ptr[rsp + 0x0], xmm0);
                asm.movdqa(__xmmword_ptr[rsp + 0x10], xmm1);
                asm.movdqa(__xmmword_ptr[rsp + 0x20], xmm2);
                asm.movdqa(__xmmword_ptr[rsp + 0x30], xmm3);
                asm.movdqa(__xmmword_ptr[rsp + 0x40], xmm4);
                asm.movdqa(__xmmword_ptr[rsp + 0x50], xmm5);
                asm.movdqa(__xmmword_ptr[rsp + 0x60], xmm6);
                asm.movdqa(__xmmword_ptr[rsp + 0x70], xmm7);
            }

            var ptr = Marshal.GetFunctionPointerForDelegate(del);
            var funcRegister = managedSource == rax ? rbx : rax;

            asm.mov(funcRegister, (ulong)ptr);
            asm.mov(managedSource, __qword_ptr[rbp - 0x78]);

            if (_isWindows)
            {
                // So-called shadow space.
                asm.sub(rsp, 0x20);
            }

            asm.call(funcRegister);
            asm.mov(__qword_ptr[rbp - 0x78], managedDest);

            if (_isWindows)
            {
                asm.add(rsp, 0x20);
            }

            if (_cpuInfo.HasXSave)
            {
                asm.mov(eax, (int)_cpuInfo.XSaveLowFlags);
                asm.mov(edx, (int)_cpuInfo.XSaveHighFlags);
                asm.xrstor(__[rsp]);
            }
            else
            {
                // Manually restore xxm:
                asm.movdqa(xmm0, __xmmword_ptr[rsp + 0x0]);
                asm.movdqa(xmm1, __xmmword_ptr[rsp + 0x10]);
                asm.movdqa(xmm2, __xmmword_ptr[rsp + 0x20]);
                asm.movdqa(xmm3, __xmmword_ptr[rsp + 0x30]);
                asm.movdqa(xmm4, __xmmword_ptr[rsp + 0x40]);
                asm.movdqa(xmm5, __xmmword_ptr[rsp + 0x50]);
                asm.movdqa(xmm6, __xmmword_ptr[rsp + 0x60]);
                asm.movdqa(xmm7, __xmmword_ptr[rsp + 0x70]);
            }

            // Now for the normal registers:
            asm.mov(rdi, __qword_ptr[rbp - 0x8]);
            asm.mov(rsi, __qword_ptr[rbp - 0x10]);
            asm.mov(rax, __qword_ptr[rbp - 0x18]);
            asm.mov(rbx, __qword_ptr[rbp - 0x20]);
            asm.mov(rcx, __qword_ptr[rbp - 0x28]);
            asm.mov(rdx, __qword_ptr[rbp - 0x30]);
            asm.mov(r8, __qword_ptr[rbp - 0x38]);
            asm.mov(r9, __qword_ptr[rbp - 0x40]);
            asm.mov(r10, __qword_ptr[rbp - 0x48]);
            asm.mov(r11, __qword_ptr[rbp - 0x50]);
            asm.mov(r12, __qword_ptr[rbp - 0x58]);
            asm.mov(r13, __qword_ptr[rbp - 0x60]);
            asm.mov(r14, __qword_ptr[rbp - 0x68]);
            asm.mov(r15, __qword_ptr[rbp - 0x70]);

            // Finally, the result:
            asm.mov(hookDest, __qword_ptr[rbp - 0x78]);

            asm.mov(rsp, rbp);
            asm.pop(rbp);
            asm.ret();

            var ms = new MemoryStream();
            asm.Assemble(new StreamCodeWriter(ms), 0);

            var arr = ms.ToArray();

            ms.Dispose();

            return arr;
        }

        private static CpuInfo GetCpuInfo()
        {
            var result = default(CpuInfo);

            var asm = new Assembler(64);

            asm.mov(r8, (ulong)&result);
            asm.mov(eax, 0x1);
            asm.cpuid();
            // Check for XSAVE flag.
            asm.and(ecx, 0x08000000);
            asm.mov(__dword_ptr[r8], ecx);
            asm.cmp(ecx, 0x0);

            var skip = asm.CreateLabel("skip");
            asm.je(skip);

            asm.mov(eax, 0x0d);
            asm.mov(ecx, 0x00);
            asm.cpuid();

            // Buffer size, Lo32, Hi32
            asm.mov(__dword_ptr[r8 + 0x4], ecx);
            asm.mov(__dword_ptr[r8 + 0x8], eax);
            asm.mov(__dword_ptr[r8 + 0xc], edx);

            asm.Label(ref skip);
            asm.ret();

            var infra = NativeFunctions.Default;
            var (mem, size) = GenerateCode(asm, infra);
            var getCpuInfo = Marshal.GetDelegateForFunctionPointer<GetCpuInfoDelegate>(mem);
            getCpuInfo();

            infra.VirtualFree(mem, size);

            return result;
        }

        private static (IntPtr, ulong) GenerateCode(Assembler asm, INativeFunctions infrastructure = null)
        {
            infrastructure = infrastructure ?? NativeFunctions.Default;

            var ms = new MemoryStream();
            // Because we don't use any global variables or functions here
            // except a few hard coded addresses to managed functions and objects,
            // we don't have to set the correct base address.
            asm.Assemble(new StreamCodeWriter(ms), 0);

            var size = AlignValueUp((ulong)ms.Length, (ulong)Environment.SystemPageSize);
            var mem = infrastructure.VirtualAllocate(IntPtr.Zero, size, MemoryProtection.Write);

            var stream = new UnmanagedMemoryStream((byte*)mem, (int)size, (int)size, FileAccess.Write);
            ms.CopyTo(stream);

            infrastructure.VirtualProtect(mem, size, MemoryProtection.Read | MemoryProtection.Execute);

            return (mem, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong AlignValueUp(ulong value, ulong alignment)
        {
            return (value + alignment - 1) & ~(alignment - 1);
        }
    }
}
