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

        public void Patch(Stream @base, Stream delta, Stream @new)
        {
            var job = new Patch(@base, delta, @new);
            job.Run();
        }

        public void GetDelta(Stream signature, Stream @new, Stream delta)
        {
            var job = new Delta(signature, @new, delta);
            job.Run();
        }
    }
}
