using System.IO;
using System.Security.Cryptography;
using LibRSync.Core.Compat;

namespace LibRSync.Core
{
    public class RDiff
    {
        public void GetSignature(Stream input, Stream signature)
        {
            var processor = new RSyncStreamSignatureWriter(signature);
            var job = new SignatureJob(input, processor);
            job.Run();
        }

        public void Patch(Stream @base, Stream delta, Stream @new)
        {
            var job = new PatchJob(@base, delta, @new);
            job.Run();
        }

        public void GetDelta(Stream signature, Stream @new, Stream delta)
        {
            var builder = new SignatureBuilder();
            var sigJob = new SignatureLoadJob(signature, builder);
            sigJob.Run();

            var processor = new DeltaEmitter(delta);

            var job = new DeltaJob(builder.GetSignature(), @new, processor);
            job.Run();
        }
    }
}
