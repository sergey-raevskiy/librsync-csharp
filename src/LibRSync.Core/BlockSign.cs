namespace LibRSync.Core
{
    public class BlockSign
    {
        public readonly uint Weak;
        public readonly StrongSum Strong;

        public BlockSign(uint weak, StrongSum strong)
        {
            Weak = weak;
            Strong = strong;
        }

        public long Start { get; set; }
        public long Length { get; set; }
    }
}
