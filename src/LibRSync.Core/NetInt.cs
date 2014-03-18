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
            long i;
            ReadInt(stream, 4, out i);

            return (int) i;
        }

        public static bool ReadInt(Stream stream, int len, out long i)
        {
            var buf = new byte[8];

            if (len > 8)
                throw new ArgumentException("len");

            if (stream.Read(buf, 0, len) != 0)
            {
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buf, 0, len);
                i = BitConverter.ToInt64(buf, 0);

                return true;
            }
            else
            {
                i = 0;
                return false;
            }
        }
    }
}
