using System.IO;

namespace LibRSync.Core.Compat
{
    internal class RSyncSignatureStreamWriter : ISignatureProcessor
    {
        private readonly Stream stream;
        private int strongLength;

        public RSyncSignatureStreamWriter(Stream stream)
        {
            this.stream = stream;
        }

        public void Header(int chunkSize, int strongLength)
        {
            NetInt.Write(stream, Const.RS_SIG_MAGIC);
            NetInt.Write(stream, chunkSize);
            NetInt.Write(stream, strongLength);

            this.strongLength = strongLength;
        }

        public void Chunk(uint weak, byte[] strong)
        {
            NetInt.Write(stream, (int)weak);
            stream.Write(strong, 0, strongLength);
        }
    }
}
