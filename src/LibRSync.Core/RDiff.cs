using System.IO;
using System.Security.Cryptography;

namespace LibRSync.Core
{
    public class RDiff
    {
        public void GetSignature(Stream input, Stream signature)
        {
            var job = new SignatureJob(input, signature);
            job.Run();
        }

        public void Patch(Stream @base, Stream delta, Stream @new)
        {
            var job = new PatchJob(@base, delta, @new);
            job.Run();
        }

        public void GetDelta(Stream signature, Stream @new, Stream delta)
        {
            var job = new DeltaJob(signature, @new, delta);
            job.Run();
        }
    }
}
