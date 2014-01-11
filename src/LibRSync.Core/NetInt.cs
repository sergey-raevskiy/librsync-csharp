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

        public static int ReadInt(Stream stream)
        {
            return (int) ReadInt(stream, 4);
        }

        public static long ReadInt(Stream stream, int len)
        {
            var buf = new byte[8];

            if (len > 8)
                throw new ArgumentException("len");

            stream.Read(buf, 0, len);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buf, 0, len);
            return BitConverter.ToInt64(buf, 0);
        }
    }
}
