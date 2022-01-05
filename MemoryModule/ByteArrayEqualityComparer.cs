using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryModule
{
    class ByteArrayEqualityComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] x, byte[] y)
        {
            return x.SequenceEqual(y);
        }

        public override int GetHashCode(byte[] obj)
        {
            int result = 0;
            unchecked
            {
                foreach (byte b in obj)
                {
                    result <<= 5;
                    result += b;
                }
            }
            return result;
        }
    }
}
