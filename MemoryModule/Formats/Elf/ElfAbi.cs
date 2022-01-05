namespace MemoryModule.Formats.Elf
{
    internal enum ElfAbi : byte
    {
        /// <summary>
        ///  It is often set to 0 regardless of the target platform. 
        /// </summary>
        SystemV = 0x00,
        Linux = 0x03
    }
}