namespace LibRSync.Core
{
    public interface ISignatureProcessor
    {
        void Header(int chunkSize, int strongLength);
        void Chunk(uint weak, StrongSum strong);
    }
}
