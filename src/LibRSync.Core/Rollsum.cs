namespace LibRSync.Core
{
    public class Rollsum
    {
        private uint s1;
        private uint s2;

        public Rollsum()
        {
            s1 = 0;
            s2 = 0;
        }

        private unsafe void DO1(byte* buf, int i)
        {
            s1 += buf[i];
            s2 += s1;
        }

        private unsafe void DO2(byte* buf, int i)
        {
            DO1(buf, i);
            DO1(buf, i + 1);
        }

        private unsafe void DO4(byte* buf, int i)
        {
            DO2(buf, i);
            DO2(buf, i + 2);
        }

        private unsafe void DO8(byte* buf, int i)
        {
            DO4(buf, i);
            DO4(buf, i + 4);
        }

        private unsafe void DO16(byte* buf)
        {
            DO8(buf, 0);
            DO8(buf, 8);
        }

        private unsafe void OF16(uint off)
        {
            s1 += 16*off;
            s2 += 136*off;
        }

        public unsafe void Update(byte[] buf, int len)
        {
            fixed (byte* p = buf)
            {
                var pbuf = p;

                while (len >= 16)
                {
                    DO16(pbuf);
                    OF16(Checksum.RS_CHAR_OFFSET);
                    pbuf += 16;
                    len -= 16;
                }

                while (len != 0)
                {
                    s1 += (*pbuf++ + Checksum.RS_CHAR_OFFSET);
                    s2 += s1;
                    len--;
                }
            }
        }

        public uint Digest
        {
            get { return (s2 << 16) + (s1 & 0xffff); }
        }
    }
}
