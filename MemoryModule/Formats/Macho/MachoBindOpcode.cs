using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    enum MachoBindOpcode : byte
    {
        Mask = 0xF0,
        ImmediateMask = 0x0F,
        Done = 0x00,
        SetDylibOrdinalImm = 0x10,
        SetDylibOrdinalUleb = 0x20,
        SetDylibSpecialImm = 0x30,
        SetSymbolTrailingFlagsImm = 0x40,
        SetTypeImm = 0x50,
        SetAddendSleb = 0x60,
        SetSegmentAndOffsetUleb = 0x70,
        AddAddrUleb = 0x80,
        DoBind = 0x90,
        DoBindAddAddrUleb = 0xA0,
        DoBindAddAddrImmScaled = 0xB0,
        DoBindUlebTimesSkippingUleb = 0xC0,
    }
}
