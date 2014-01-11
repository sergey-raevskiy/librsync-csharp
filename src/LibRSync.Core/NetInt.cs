using System;
using System.IO;

namespace LibRSync.Core
{
    internal class NetInt
    {
        public static void Write(Stream stream, int i)
        {
            var bytes = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
