using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.MacOS.Macho
{
    unsafe class MachoRebaseCollection : MachoCompressedCollection<MachoRebase>
    {
        const byte OpMask = (byte)MachoRebaseOpcode.Mask;
        const byte ImMask = (byte)MachoRebaseOpcode.ImmediateMask;

        private static readonly ulong PtrSize = (ulong)sizeof(IntPtr);

        public MachoRebaseCollection(byte* memory, ulong offset, ulong size) : base(memory, offset, size)
        {
        }

        protected override List<MachoRebase> Decompress()
        {
            var ptr = _data;
            var end = _data + _size;

            var result = new List<MachoRebase>();

            var currentRebase = new MachoRebase();

            while (ptr < end)
            {
                var op = (MachoRebaseOpcode)((*ptr) & OpMask);
                var im = (byte)((*ptr) & ImMask);
                ++ptr;

                switch (op)
                {
                    case MachoRebaseOpcode.Done:
                        currentRebase = new MachoRebase();
                        break;
                    case MachoRebaseOpcode.AddAddrImmScaled:
                        currentRebase.SegmentOffset += PtrSize * im;
                        break;
                    case MachoRebaseOpcode.AddAddrUleb:
                        currentRebase.SegmentOffset += ReadUleb128(ref ptr);
                        break;
                    case MachoRebaseOpcode.SetSegmentAndOffsetUleb:
                        currentRebase.SegmentIndex = im;
                        currentRebase.SegmentOffset = ReadUleb128(ref ptr);
                        break;
                    case MachoRebaseOpcode.SetTypeImm:
                        currentRebase.Type = (MachoRebaseType)im;
                        break;
                    case MachoRebaseOpcode.DoRebaseAddAddrUleb:
                        result.Add((MachoRebase)currentRebase.Clone());
                        currentRebase.SegmentOffset += ReadUleb128(ref ptr);
                        break;
                    case MachoRebaseOpcode.DoRebaseImmTimes:
                        // Bruh, this is faster than a Linq
                        for (int i = 0; i < im; ++i)
                        {
                            result.Add((MachoRebase)currentRebase.Clone());
                            // The pointer should go forward, that makes more sense.
                            currentRebase.SegmentOffset += PtrSize;
                        }
                        break;
                    case MachoRebaseOpcode.DoRebaseUlebTimes:
                    { 
                        ulong count = ReadUleb128(ref ptr);
                        for (ulong i = 0; i < count; ++i)
                        {
                            result.Add((MachoRebase)currentRebase.Clone());
                            currentRebase.SegmentOffset += PtrSize;
                        }
                    }
                    break;
                    case MachoRebaseOpcode.DoRebaseUlebTimesSkippingUleb:
                    {
                        ulong count = ReadUleb128(ref ptr);
                        ulong skip = ReadUleb128(ref ptr);
                        for (ulong i = 0; i < count; ++i)
                        {
                            result.Add((MachoRebase)currentRebase.Clone());
                            currentRebase.SegmentOffset += skip;
                        }
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
