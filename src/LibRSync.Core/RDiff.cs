using System.IO;
using System.Security.Cryptography;

namespace LibRSync.Core
{
    public class RDiff
    {
        public void GetSignature(Stream input, Stream signature)
        {
            var job = new Signature(input, signature);
            job.Run();
        }
    }
}
