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
}
