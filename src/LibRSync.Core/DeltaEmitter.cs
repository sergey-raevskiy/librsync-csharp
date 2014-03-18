using System.IO;

namespace LibRSync.Core
{
    internal class DeltaEmitter : IDeltaProcessor
    {
        private readonly Stream delta;

        public DeltaEmitter(Stream delta)
        {
            this.delta = delta;
        }

        public void Copy(long start, long length)
        {

        }

        public void Literal(byte[] data, long offset, long count)
        {

        }
    }
}
