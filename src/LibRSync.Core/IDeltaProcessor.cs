namespace LibRSync.Core
{
    interface IDeltaProcessor
    {
        void Copy(long start, long length);
        void Literal(byte[] data, long offset, long count);
    }
}
