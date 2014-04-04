using System;
using System.IO;

namespace LibRSync.Core
{
    internal class NetInt
    {
        public static void Write(Stream stream, int i)
        {
            Write(stream, i, 4);
        }

        public static void Write(Stream stream, long i, int len)
        {
            var bytes = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            if (len > 8)
                throw new ArgumentException("len");

            stream.Write(bytes, 8 - len, len);
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
