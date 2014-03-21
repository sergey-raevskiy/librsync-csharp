namespace LibRSync.Core
{
    public interface IStrongHashAlgorithm
    {
        StrongSum GetSum(byte[] buf, int offset, int len);
    }
}
