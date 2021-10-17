using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    enum MachoSectionType : uint
    {
        Regular = 0x0,
        Zerofill = 0x1,
        CstringLiterals = 0x2,
        _4ByteLiterals = 0x3,
        _8ByteLiterals = 0x4,
        LiteralPointers = 0x5,
        NonLazySymbolPointers = 0x6,
        LazySymbolPointers = 0x7,
        SymbolStubs = 0x8,
        ModInitFuncPointers = 0x9,
        ModTermFuncPointers = 0xa,
        Coalesced = 0xb,
        GBZerofile = 0xc,
        Interposing = 0xd,
        _16ByteLiterals = 0xe,
        DtraceDOF = 0xf,
        LazyDylibSymbolPointers = 0x10,
        ThreadLocalRegular = 0x11,
        ThreadLocalZeroFill = 0x12,
        ThreadLocalVariables = 0x13,
        ThreadLocalVariablePointers = 0x14,
        ThreadLocalInitFunctionPointers = 0x15
    }
}
