using System.IO;

namespace LibRSync.Core
{
    internal class Signature : Job
    {
        private const int RS_SIG_MAGIC = 0x72730136;

        private Stream input;
        private Stream signature;

        private byte[] chunk = new byte[2048];

        public Signature(Stream input, Stream signature)
            : base("signature")
        {
            this.input = input;
            this.signature = signature;
        }

        protected override StateFunc InitialState()
        {
            return Header;
        }

        private StateFunc Header()
        {
            NetInt.Write(signature, RS_SIG_MAGIC);
            NetInt.Write(signature, 2048);
            NetInt.Write(signature, 8);

            return Generate;
        }

        private StateFunc Generate()
        {
            var len = input.Read(chunk, 0, chunk.Length);
            if (len < 1) return Completed;

            var weak = Checksum.Weak(chunk, len);
            NetInt.Write(signature, (int) weak);

            var strong = Checksum.Strong(chunk, len);
            signature.Write(strong, 0, 8);

            return Generate;
        }
    }
}
