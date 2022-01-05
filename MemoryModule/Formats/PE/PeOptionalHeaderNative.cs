using System;
using System.Runtime.InteropServices;

namespace MemoryModule.Formats.PE
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct PeOptionalHeaderNative
    {
        public const int NumberOfDirectoryEntries = 16;

        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;

        // In C#, we don't need macros.
        // We have more evil stuff.
        [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 1)]
        private struct DUMMY8BYTEVALUE
        {
            // 32 bit fields
            [FieldOffset(0)]
            public uint BaseOfData;
            [FieldOffset(4)]
            public uint ImageBase32;
            // 64 bit fields
            [FieldOffset(0)]
            public ulong ImageBase64;
        }
        private DUMMY8BYTEVALUE _architectureSpecificValue1;

        public uint BaseOfData
        {
            get => _architectureSpecificValue1.BaseOfData;
            set => _architectureSpecificValue1.BaseOfData = value;
        }
        public UIntPtr ImageBase
        {
            get => Environment.Is64BitProcess ?
                (UIntPtr)_architectureSpecificValue1.ImageBase64 :
                (UIntPtr)_architectureSpecificValue1.ImageBase32;
            set
            {
                if (Environment.Is64BitProcess)
                {
                    _architectureSpecificValue1.ImageBase64 = (ulong)value;
                }
                else
                {
                    _architectureSpecificValue1.ImageBase32 = (uint)value;
                }
            }
        }

        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public UIntPtr SizeOfStackReserve;
        public UIntPtr SizeOfStackCommit;
        public UIntPtr SizeOfHeapReserve;
        public UIntPtr SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;

        public fixed ulong DataDirectoryBuffer[NumberOfDirectoryEntries];
    }
}