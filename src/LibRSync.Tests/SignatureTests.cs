﻿using System.IO;
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
    }
}
