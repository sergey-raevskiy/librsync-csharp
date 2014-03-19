using System.IO;

namespace LibRSync.Core
{
    public class SignatureJob : Job
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
