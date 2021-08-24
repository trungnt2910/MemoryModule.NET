using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule
{
    public enum Error : uint
    {
        InvalidData = 13,
        BadExeFormat = 193,
        OutOfMemory = 14,
        DllInitFailed = 1114,
        ModuleNotFound = 126,
        ProcNotFound = 127,
    }
}
