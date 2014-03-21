namespace LibRSync.Core
{
    public class StrongHashAlgorithm
    {
        public static readonly IStrongHashAlgorithm Md4 = new Md4HashAlgorithm();
    }
}
