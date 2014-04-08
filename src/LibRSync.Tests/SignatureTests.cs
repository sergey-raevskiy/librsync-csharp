using System;
using System.IO;
using LibRSync.Core;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class SignatureTests : TestBase
    {
        [Test]
        public void BasicTest()
        {
            var input = GetTestDataStream("signature.input.01.in");
            var expected = StreamToArray(GetTestDataStream("signature.input.01.sig"));

            var rdiff = new RDiff();

            using (var ms = new MemoryStream())
            {
                rdiff.GetSignature(input, ms);
                var actual = ms.ToArray();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void LoadTest()
        {
            using (var sig = GetTestDataStream("signature.input.01.sig"))
            {
                var builder = new SignatureBuilder();

                var job = new SignatureLoadJob(sig, builder);
                job.Run();

                var signature = builder.GetSignature();

                Assert.AreEqual(2048, signature.ChunkSize);
                Assert.AreEqual(8, signature.StrongLength);
            }
        }

        public void SameBlockTest()
        {
            var r = new Random(1337);
            var b = new byte[2048];
            r.NextBytes(b);

            using (var ms = new MemoryStream())
            {
                ms.Write(b, 0, b.Length);
                ms.Write(b, 0, b.Length);
                ms.Seek(0, SeekOrigin.Begin);

                var builder = new SignatureBuilder();
                var signatureJob = new SignatureJob(ms, builder);
                signatureJob.Run();

                var sig = builder.GetSignature();
            }
        }
    }
}
