using System.IO;

namespace LibRSync.Core
{
    internal class StreamProcessor : ISignatureProcessor
    {
        private readonly Stream stream;
        private int strongLength;

        public StreamProcessor(Stream stream)
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

        public void Chunk(long weak, byte[] strong)
        {
            NetInt.Write(stream, (int)weak);
            stream.Write(strong, 0, strongLength);
        }
    }

    internal class SignatureJob : Job
    {
        private Stream input;
        private ISignatureProcessor processor;

        private byte[] chunk = new byte[2048];

        public SignatureJob(Stream input, ISignatureProcessor processor)
            : base("signature")
        {
            this.input = input;
            this.processor = processor;
        }

        protected override StateFunc InitialState()
        {
            return Header;
        }

        private StateFunc Header()
        {
            processor.Header(2048, 8);

            return Generate;
        }

        private StateFunc Generate()
        {
            var len = input.Read(chunk, 0, chunk.Length);
            if (len < 1) return Completed;

            var weak = Checksum.Weak(chunk, len);
            var strong = Checksum.Strong(chunk, len);

            processor.Chunk(weak, strong);

            return Generate;
        }
    }
}
