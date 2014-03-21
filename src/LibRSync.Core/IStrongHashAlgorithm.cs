namespace LibRSync.Core
{
    public interface IStrongHashAlgorithm
    {
        StrongSum GetSum(byte[] buf, int offset, int len);
    }

    public class StrongHashAlgorithm
    {
        public static readonly IStrongHashAlgorithm Md4 = new Md4HashAlgorithm();
    }

    internal class Md4HashAlgorithm : IStrongHashAlgorithm
    {
        public StrongSum GetSum(byte[] buf, int offset, int len)
        {
            using (var md4 = new Md4())
            {
                return new StrongSum(md4.ComputeHash(buf, offset, len), this);
            }
        }
    }
}
