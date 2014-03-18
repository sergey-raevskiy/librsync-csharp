using System.Collections.Generic;
using System.Linq;

namespace LibRSync.Core
{
    public struct BlockSign
    {
        public readonly long Weak;
        public readonly byte[] Strong;

        public BlockSign(long weak, byte[] strong)
        {
            Weak = weak;
            Strong = strong;
        }
    }

    public class Signature
    {
        private Dictionary<long, List<BlockSign>> blocks;

        public Signature(int chunkSize,
                         int strongLength,
                         IEnumerable<BlockSign> blocks)
        {
            ChunkSize = chunkSize;
            StrongLength = strongLength;

            this.blocks = blocks.GroupBy(b => b.Weak)
                                .ToDictionary(g => g.Key,
                                              g => g.ToList());
        }

        public int ChunkSize { get; private set; }
        public int StrongLength { get; private set; }
    }

    public class SignatureBuilder : ISignatureProcessor
    {
        private readonly List<BlockSign> blocks = new List<BlockSign>();

        private int chunkSize;
        private int strongLength;

        void ISignatureProcessor.Header(int chunkSize, int strongLength)
        {
            this.chunkSize = chunkSize;
            this.strongLength = strongLength;

            blocks.Clear();
        }

        void ISignatureProcessor.Chunk(long weak, byte[] strong)
        {
            var blockSign = new BlockSign(weak, strong);

            blocks.Add(blockSign);
        }

        public Signature GetSignature()
        {
            return new Signature(chunkSize, strongLength, blocks);
        }
    }
}
