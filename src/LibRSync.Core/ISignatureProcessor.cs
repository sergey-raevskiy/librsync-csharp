namespace LibRSync.Core
{
    interface ISignatureProcessor
    {
        void Header(int chunkSize, int strongLength);
        void Chunk(long weak, byte[] strong);
    }
}
