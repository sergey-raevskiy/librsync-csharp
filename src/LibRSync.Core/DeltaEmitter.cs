using System.IO;

namespace LibRSync.Core
{
    public class DeltaEmitter : IDeltaProcessor
    {
        private readonly Stream delta;

        public DeltaEmitter(Stream delta)
        {
            this.delta = delta;
        }

        private int GetIntLen(long i)
        {
            if (i <= 0xff)
                return 1;
            else if (i <= 0xffff)
                return 2;
            else if (i <= 0xffffff)
                return 4;
            else
                return 8;
        }

        public void Header()
        {
            NetInt.Write(delta, Const.RS_DELTA_MAGIC);
        }

        public void Copy(long start, long length)
        {
            var lStart = GetIntLen(start);
            var lLength = GetIntLen(length);

            var op = ((lStart << 2) + lLength) + 64; // MAGIC

            delta.WriteByte((byte) op);
            NetInt.Write(delta, start, lStart);
            NetInt.Write(delta, length, lLength);
        }

        public void Literal(byte[] data, long offset, long count)
        {
            if (count <= 64)
            {
                delta.WriteByte((byte) count);
            }
            else
            {
                var lCount = GetIntLen(count);
                var op = lCount + 64;
                delta.WriteByte((byte)op);
                NetInt.Write(delta, count, lCount);
            }

            delta.Write(data, (int) offset, (int) count);
        }
    }
}
