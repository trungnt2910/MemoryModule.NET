using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule.Formats.PE
{
    internal enum DllOperation : uint
    {
        ProcessAttach = 1,
        ThreadAttach = 2,
        ThreadDetach = 3,
        ProcessDetach = 0,
        ProcessVerifier = 4
    }
}
