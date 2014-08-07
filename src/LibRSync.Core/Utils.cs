namespace LibRSync.Core
{
    public static class Utils
    {
        public static int GetIntLen(long i)
        {
            if (i <= 0xff)
                return 1;
            else if (i <= 0xffff)
                return 2;
            else if (i <= 0xffffff)
                return 4;
            else
                return 8;
        }
    }
}
