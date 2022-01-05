using System;
using System.Collections.Generic;
using System.Text;

namespace MemoryModule.Formats.Macho
{
    unsafe class MachoExportCollection : MachoCompressedCollection<MachoExport>
    {
        public MachoExportCollection(byte* memory, ulong offset, ulong size) : base(memory, offset, size)
        {
        }

        protected override List<MachoExport> Decompress()
        {
            var result = new List<MachoExport>();
            var builder = new StringBuilder();

            Decompress(_data, result, builder);

            System.Diagnostics.Debug.Assert(builder.Length == 0);

            return result;
        }

        private void Decompress(byte* ptr, List<MachoExport> result, StringBuilder prefix)
        {
            // Nodes for a symbol start with a uleb128 that is the length of
            // the exported symbol information for the string so far.
            var length = ReadUleb128(ref ptr);

            // If there is no exported symbol, the node starts with a zero byte. 
            if (length != 0)
            {
                var currentExport = new MachoExport();

                currentExport.Name = prefix.ToString();

                // If there is exported info, it follows the length.  First is
                // a uleb128 containing flags.
                currentExport.Flags = (MachoExportSymbolFlags)ReadUleb128(ref ptr);


                // If the flags
                // is EXPORT_SYMBOL_FLAGS_REEXPORT, then following the flags is
                // a uleb128 encoded library ordinal, then a zero terminated
                // UTF8 string.
                if (currentExport.Flags.HasFlag(MachoExportSymbolFlags.Reexport))
                {
                    currentExport.LibraryOrdinal = ReadUleb128(ref ptr);
                    currentExport.ReexportName = ReadUtf8(ref ptr);
                    // If the string is zero length, then the symbol
                    // is re-export from the specified dylib with the same name.
                    if (string.IsNullOrEmpty(currentExport.ReexportName))
                    {
                        currentExport.ReexportName = currentExport.Name;
                    }
                }
                // Normally, it is followed by a
                // uleb128 encoded offset which is location of the content named
                // by the symbol from the mach_header for the image.
                else
                {
                    currentExport.Location = ReadUleb128(ref ptr);
                }

                result.Add(currentExport);
            }

            // After the optional exported symbol information is a byte of
            // how many edges(0 - 255) that this node has leaving it,
            var edges = *ptr;
            ++ptr;
            // followed by each edge.
            for (int i = 0; i < edges; ++i)
            {
                // Each edge is a zero terminated UTF8 of the addition chars
                // in the symbol,
                var additionalString = ReadUtf8(ref ptr);
                prefix.Append(additionalString);
                // followed by a uleb128 offset for the node that
                // edge points to.
                var nodeOffset = _data + ReadUleb128(ref ptr);
                
                Decompress(nodeOffset, result, prefix);
                
                prefix.Remove(prefix.Length - additionalString.Length, additionalString.Length);
            }
        }
    }
}
