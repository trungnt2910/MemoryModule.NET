using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule.Windows
{
    public static class Image
    {
        public const int DosSignature = 0x5A4D;
        public const int NTSignature = 0x00004550;
        public const int FileMachinei386 = 0x14c;
        public const int FileMachineAMD64 = 0x8664;
        public const int FileDll = 0x2000;
        public const int SizeOfBaseRelocation = 8;
        public const ulong OrdinalFlag64 = 0x8000000000000000;
        public const uint OrdinalFlag32 = 0x80000000;
        public const int SectionCntInitializedData = 0x00000040;
        public const int SectionCntUninitializedData = 0x00000080;
    }
}
