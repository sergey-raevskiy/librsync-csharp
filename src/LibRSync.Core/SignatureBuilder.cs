using System.Collections.Generic;
using System.Linq;

namespace LibRSync.Core
{
    public class BlockSign
    {
        public readonly uint Weak;
        public readonly byte[] Strong;

        public BlockSign(uint weak, byte[] strong)
        {
            Weak = weak;
            Strong = strong;
        }

        public long Start { get; set; }
        public long Length { get; set; }
    }

    public class Signature
    {
        private Dictionary<uint, List<BlockSign>> blocks;

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

        public bool HasWeak(uint weak)
        {
            return blocks.ContainsKey(weak);
        }

        public BlockSign LookupBlock(uint weak, byte[] strong)
        {
            List<BlockSign> list;
            if (blocks.TryGetValue(weak, out list))
            {
                return list.SingleOrDefault(b => b.Strong.SequenceEqual(strong));
            }
            else
            {
                return null;
            }
        }
    }

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

        void ISignatureProcessor.Chunk(uint weak, byte[] strong)
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
