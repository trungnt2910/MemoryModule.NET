using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.PE
{
    enum PeBaseRelocationType : uint
    {
        /// <summary>
        /// The base relocation is skipped. This type can be used to pad a block.
        /// </summary>
        Absolute = 0,
        /// <summary>
        /// The base relocation adds the high 16 bits of the difference to the 16-bit field at offset. The 16-bit field represents the high value of a 32-bit word.
        /// </summary>
        High = 1,
        /// <summary>
        /// The base relocation adds the low 16 bits of the difference to the 16-bit field at offset. The 16-bit field represents the low half of a 32-bit word.
        /// </summary>
        Low = 2,
        /// <summary>
        /// The base relocation applies all 32 bits of the difference to the 32-bit field at offset.
        /// </summary>
        Highlow = 3,
        /// <summary>
        /// The base relocation adds the high 16 bits of the difference to the 16-bit field at offset. The 16-bit field represents the high value of a 32-bit word. The low 16 bits of the 32-bit value are stored in the 16-bit word that follows this base relocation. This means that this base relocation occupies two slots.
        /// </summary>
        Highadj = 4,
        /// <summary>
        /// The relocation interpretation is dependent on the machine type. When the machine type is MIPS, the base relocation applies to a MIPS jump instruction.
        /// </summary>
        MipsJmpaddr = 5,
        /// <summary>
        /// This relocation is meaningful only when the machine type is ARM or Thumb. The base relocation applies the 32-bit address of a symbol across a consecutive MOVW/MOVT instruction pair.
        /// </summary>
        ArmMov32 = 5,
        /// <summary>
        /// This relocation is only meaningful when the machine type is RISC-V. The base relocation applies to the high 20 bits of a 32-bit absolute address.
        /// </summary>
        RiscvHigh20 = 5,
        /// <summary>
        /// This relocation is meaningful only when the machine type is Thumb. The base relocation applies the 32-bit address of a symbol to a consecutive MOVW/MOVT instruction pair.
        /// </summary>
        ThumbMov32 = 7,
        /// <summary>
        /// This relocation is only meaningful when the machine type is RISC-V. The base relocation applies to the low 12 bits of a 32-bit absolute address formed in RISC-V I-type instruction format.
        /// </summary>
        RiscvLow12i = 7,
        /// <summary>
        /// This relocation is only meaningful when the machine type is RISC-V. The base relocation applies to the low 12 bits of a 32-bit absolute address formed in RISC-V S-type instruction format.
        /// </summary>
        RiscvLow12s = 8,
        /// <summary>
        /// The relocation is only meaningful when the machine type is MIPS. The base relocation applies to a MIPS16 jump instruction.
        /// </summary>
        MipsJmpaddr16 = 9,
        /// <summary>
        /// The base relocation applies the difference to the 64-bit field at offset.
        /// </summary>
        Dir64 = 10,
    }
}
