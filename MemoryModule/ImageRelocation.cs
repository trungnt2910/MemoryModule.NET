using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule
{
    internal enum ImageRelocation
    {
        BasedAbsolute = 0,
        BasedHigh = 1,
        BasedLow = 2,
        BasedHighLow = 3,
        BasedHighAdj = 4,
        BasedDir64 = 10
    }
}
