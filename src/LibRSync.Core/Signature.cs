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

            this.blocks = new Dictionary<StrongSum, BlockSign>();
            foreach (var block in blocks)
            {
                if (!this.blocks.ContainsKey(block.Strong))
                    this.blocks.Add(block.Strong, block);
            }

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
