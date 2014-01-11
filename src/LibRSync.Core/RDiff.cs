using System.IO;
using System.Security.Cryptography;

namespace LibRSync.Core
{
    public class RDiff
    {
        private const int RS_SIG_MAGIC = 0x72730136;

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

                var weak = Checksum.Weak(buf, len);
                NetInt.Write(signature, (int) weak);

                var strong = Checksum.Strong(buf, len);
                signature.Write(strong, 0, 8);
            }
        }
    }
}
