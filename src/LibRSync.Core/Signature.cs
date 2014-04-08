using System.Collections.Generic;
using System.Linq;

namespace LibRSync.Core
{
    public class Signature
    {
        private Dictionary<StrongSum, BlockSign> blocks;
        private HashSet<uint> weaks;

        public Signature(int chunkSize,
            int strongLength,
            IEnumerable<BlockSign> blocks)
        {
            ChunkSize = chunkSize;
            StrongLength = strongLength;

            this.blocks = blocks.ToDictionary(b => b.Strong);

            this.weaks = new HashSet<uint>();
            foreach (var block in this.blocks.Values)
            {
                weaks.Add(block.Weak);
            }
        }

        public int ChunkSize { get; private set; }
        public int StrongLength { get; private set; }

        public bool HasWeak(uint weak)
        {
            return weaks.Contains(weak);
        }

        public BlockSign LookupBlock(uint weak, StrongSum strong)
        {
            BlockSign block;

            if (blocks.TryGetValue(strong, out block))
            {
                return block;
            }
            else
            {
                return null;
            }
        }
    }
}
