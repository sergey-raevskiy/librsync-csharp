using System.Collections.Generic;
using System.Linq;

namespace LibRSync.Core
{
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
}
