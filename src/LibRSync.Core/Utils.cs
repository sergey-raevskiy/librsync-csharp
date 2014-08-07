namespace LibRSync.Core
{
    public static class Utils
    {
        public static int GetIntLen(long i)
        {
            if ((i & ~0xffL) == 0)
                return 1;
            else if ((i & ~0xffffL) == 0)
                return 2;
            else if ((i & ~0xffffffffL) == 0)
                return 4;
            else
                return 8;
        }
    }
}
