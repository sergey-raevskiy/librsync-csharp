using System.IO;
using System.Security.Cryptography;

namespace LibRSync.Core
{
    public class RDiff
    {
        private const uint RS_CHAR_OFFSET = 31;
        private const int RS_SIG_MAGIC = 0x72730136;

        private uint CalcWeakSum(byte[] buf, int len)
        {
            var s1 = 0u;
            var s2 = 0u;

            int i;

            for (i = 0; i < (len - 4); i += 4)
            {
                s2 += 4*(s1 + buf[i]) + (uint) 3*buf[i + 1] +
                      (uint) 2*buf[i + 2] + buf[i + 3] + 10*RS_CHAR_OFFSET;

                s1 += (uint) (buf[i + 0] + buf[i + 1] + buf[i + 2] + buf[i + 3] +
                              4*RS_CHAR_OFFSET);
            }

            for (; i < len; i++)
            {
                s1 += (buf[i] + RS_CHAR_OFFSET);
                s2 += s1;
            }

            return (s1 & 0xffff) + (s2 << 16);
        }

        public void GetSignature(Stream input, Stream signature)
        {
            NetInt.Write(signature, RS_SIG_MAGIC);
            NetInt.Write(signature, 2048);
            NetInt.Write(signature, 8);

            var buf = new byte[2048];

            while (true)
            {
                var len = input.Read(buf, 0, buf.Length);
                if (len < 1) break;

                var weak = CalcWeakSum(buf, len);
                NetInt.Write(signature, (int) weak);

                using (HashAlgorithm hash = new Md4())
                {
                    var md4 = hash.ComputeHash(buf, 0, len);
                    signature.Write(md4, 0, 8);
                }
            }
        }
    }
}
