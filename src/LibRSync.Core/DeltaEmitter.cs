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

        public void Header()
        {
            NetInt.Write(delta, Const.RS_DELTA_MAGIC);
        }

        public void Copy(long start, long length)
        {
            var lStart = Utils.GetIntLen(start);
            var lLength = Utils.GetIntLen(length);

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
                var lCount = Utils.GetIntLen(count);
                var op = lCount + 64;
                delta.WriteByte((byte)op);
                NetInt.Write(delta, count, lCount);
            }

            delta.Write(data, (int) offset, (int) count);
        }

        public void End()
        {
            delta.WriteByte((byte) OpKind.End);
        }
    }
}
