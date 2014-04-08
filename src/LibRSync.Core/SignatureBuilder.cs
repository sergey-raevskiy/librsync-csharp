using System.Collections.Generic;

namespace LibRSync.Core
{
    public class SignatureBuilder : ISignatureProcessor
    {
        private readonly List<BlockSign> blocks = new List<BlockSign>();

        private int chunkSize;
        private int strongLength;
        private long position;

        void ISignatureProcessor.Header(int chunkSize, int strongLength)
        {
            this.chunkSize = chunkSize;
            this.strongLength = strongLength;
            position = 0;

            blocks.Clear();
        }

        void ISignatureProcessor.Chunk(uint weak, StrongSum strong)
        {
            var blockSign = new BlockSign(weak, strong)
            {
                Start = position,
                Length = chunkSize
            };

            blocks.Add(blockSign);

            position += chunkSize;
        }

        public Signature GetSignature()
        {
            return new Signature(chunkSize, strongLength, blocks);
        }
    }
}
