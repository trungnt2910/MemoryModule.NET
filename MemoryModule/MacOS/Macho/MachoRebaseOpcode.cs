using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    enum MachoRebaseOpcode
    {
        Mask = 0xF0,
        ImmediateMask = 0x0F,
        Done = 0x00,
        SetTypeImm = 0x10,
        SetSegmentAndOffsetUleb = 0x20,
        AddAddrUleb = 0x30,
        AddAddrImmScaled = 0x40,
        DoRebaseImmTimes = 0x50,
        DoRebaseUlebTimes = 0x60,
        DoRebaseAddAddrUleb = 0x70,
        DoRebaseUlebTimesSkippingUleb = 0x80,
    }
}
