using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    unsafe class MachoBindCollection : MachoCompressedCollection<MachoBind>
    {
        const byte OpMask = (byte)MachoBindOpcode.Mask;
        const byte ImMask = (byte)MachoBindOpcode.ImmediateMask;

        private static readonly ulong PtrSize = (ulong)sizeof(IntPtr);

        public MachoBindCollection(byte* memory, ulong offset, ulong size) : base(memory, offset, size)
        {
        }

        protected override List<MachoBind> Decompress()
        {
            var ptr = _data;
            var end = _data + _size;

            var result = new List<MachoBind>();

            var currentBind = new MachoBind();

            while (ptr < end)
            {
                var op = (MachoBindOpcode)((*ptr) & OpMask);
                var im = (byte)((*ptr) & ImMask);
                ++ptr;

                switch (op)
                {
                    case MachoBindOpcode.Done:
                        currentBind = new MachoBind();
                        break;
                    case MachoBindOpcode.AddAddrUleb:
                        var addr = ReadUleb128(ref ptr);
                        currentBind.SegmentOffset += addr;
                        break;
                    case MachoBindOpcode.SetAddendSleb:
                        var addend = ReadUleb128(ref ptr);
                        currentBind.Addend = addend;
                        break;
                    case MachoBindOpcode.SetDylibOrdinalImm:
                        currentBind.LibraryOrdinal = im;
                        break;
                    case MachoBindOpcode.SetDylibOrdinalUleb:
                        currentBind.LibraryOrdinal = ReadUleb128(ref ptr);
                        break;
                    case MachoBindOpcode.SetDylibSpecialImm:
                        currentBind.LibraryOrdinal = (im == 0) ? 0ul : (byte)(OpMask | im);
                        break;
                    case MachoBindOpcode.SetSegmentAndOffsetUleb:
                        currentBind.SegmentIndex = im;
                        currentBind.SegmentOffset = ReadUleb128(ref ptr);
                        break;
                    case MachoBindOpcode.SetSymbolTrailingFlagsImm:
                        currentBind.Name = ReadUtf8(ref ptr);
                        break;
                    case MachoBindOpcode.SetTypeImm:
                        currentBind.Type = (MachoBindType)im;
                        break;
                    case MachoBindOpcode.DoBind:
                        result.Add((MachoBind)currentBind.Clone());
                        currentBind.SegmentOffset += PtrSize;
                        break;
                    case MachoBindOpcode.DoBindAddAddrImmScaled:
                        result.Add((MachoBind)currentBind.Clone());
                        currentBind.SegmentOffset += im * PtrSize;
                        break;
                    case MachoBindOpcode.DoBindAddAddrUleb:
                        result.Add((MachoBind)currentBind.Clone());
                        currentBind.SegmentOffset += ReadUleb128(ref ptr);
                        break;
                    case MachoBindOpcode.DoBindUlebTimesSkippingUleb:
                        var count = ReadUleb128(ref ptr);
                        var skip = ReadUleb128(ref ptr);

                        for (ulong i = 0; i < count; ++i)
                        {
                            result.Add((MachoBind)currentBind.Clone());
                            currentBind.SegmentOffset += skip;
                        }

                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"Unknown opcode: 0x{(ulong)op:x}");
                        break;
                }
            }

            return result;
        }
    }
}
