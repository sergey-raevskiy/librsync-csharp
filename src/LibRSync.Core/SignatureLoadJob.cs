using System;
using System.IO;

namespace LibRSync.Core
{
    public class SignatureLoadJob : Job
    {
        private readonly Stream stream;
        private readonly ISignatureProcessor processor;

        private byte[] strong;

        public SignatureLoadJob(Stream stream, ISignatureProcessor processor)
            : base("loadsig")
        {
            this.stream = stream;
            this.processor = processor;
        }

        protected override StateFunc InitialState()
        {
            return Magic;
        }

        private StateFunc Magic()
        {
            if (NetInt.ReadInt(stream) != Const.RS_SIG_MAGIC)
            {
                throw new Exception("Bad signature");
            }

            return Header;
        }

        private StateFunc Header()
        {
            var blockLength = NetInt.ReadInt(stream);
            var strongLength = NetInt.ReadInt(stream);

            processor.Header(blockLength, strongLength);

            strong = new byte[strongLength];

            return Read;
        }

        private StateFunc Read()
        {
            long weak;

            if (!NetInt.ReadInt(stream, 4, out weak))
                return Completed;

            stream.Read(strong, 0, strong.Length);

            processor.Chunk((uint)weak, strong);

            return Read;
        }
    }
}
