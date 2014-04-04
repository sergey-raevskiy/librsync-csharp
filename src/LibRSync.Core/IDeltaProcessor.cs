namespace LibRSync.Core
{
    public interface IDeltaProcessor
    {
        void Header();
        void Copy(long start, long length);
        void Literal(byte[] data, long offset, long count);
        void End();
    }
}
