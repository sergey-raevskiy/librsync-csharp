namespace LibRSync.Core
{
    public interface IStrongSumAlgrorithm
    {
        StrongSum GetSum(byte[] buf, int offset, int len);
    }
}
