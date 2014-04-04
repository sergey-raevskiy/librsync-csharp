using System;
using System.IO;
using LibRSync.Core;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class DeltaTests : TestBase
    {
        [TestCase("changes.input.01.in", "changes.input.01.in")]
        [TestCase("changes.input.01.in", "changes.input.02.in")]
        [TestCase("changes.input.01.in", "changes.input.03.in")]
        [TestCase("changes.input.01.in", "changes.input.04.in")]
        [TestCase("changes.input.02.in", "changes.input.01.in")]
        [TestCase("changes.input.02.in", "changes.input.02.in")]
        [TestCase("changes.input.02.in", "changes.input.03.in")]
        [TestCase("changes.input.02.in", "changes.input.04.in")]
        [TestCase("changes.input.03.in", "changes.input.01.in")]
        [TestCase("changes.input.03.in", "changes.input.02.in")]
        [TestCase("changes.input.03.in", "changes.input.03.in")]
        [TestCase("changes.input.03.in", "changes.input.04.in")]
        [TestCase("changes.input.04.in", "changes.input.01.in")]
        [TestCase("changes.input.04.in", "changes.input.02.in")]
        [TestCase("changes.input.04.in", "changes.input.03.in")]
        [TestCase("changes.input.04.in", "changes.input.04.in")]
        public void TripleTest(string oldName, string newName)
        {
            using (var old = GetTestDataStream(oldName))
            using (var old2 = GetTestDataStream(oldName))
            using (var @new = GetTestDataStream(newName))
            using (var actual = new MemoryStream())
            {
                var builder = new SignatureBuilder();
                var sigJob = new SignatureJob(old, builder);
                sigJob.Run();

                old.Seek(0, SeekOrigin.Begin);

                using (var delta = new MemoryStream())
                {
                    var deltaEmitter = new DeltaEmitter(delta);
                    var deltaJob = new DeltaJob(builder.GetSignature(), @new, deltaEmitter);
                    deltaJob.Run();

                    delta.Seek(0, SeekOrigin.Begin);

                    var patchJob = new PatchJob(old2, delta, actual);
                    patchJob.Run();
                }

                actual.Seek(0, SeekOrigin.Begin);
                @new.Seek(0, SeekOrigin.Begin);

                Assert.AreEqual(StreamToArray(@new), StreamToArray(actual));
            }
        }

        private class HumanReadableDeltaWriter : IDeltaProcessor
        {
            private StringWriter writer = new StringWriter();

            public void Header()
            {
                writer.WriteLine("HEADER");
            }

            public void Copy(long start, long length)
            {
                writer.WriteLine("COPY {0} {1}", start, length);
            }

            public void Literal(byte[] data, long offset, long count)
            {
                writer.WriteLine("LITERAL {0}", count);
            }

            public void End()
            {
                writer.WriteLine("END");
            }

            public override string ToString()
            {
                return writer.ToString();
            }
        }

        private string GetHumanReadableDelta(string oldFile, string newFile)
        {
            using (var oldStream = GetTestDataStream(oldFile))
            using (var newStream = GetTestDataStream(newFile))
            {
                var builder = new SignatureBuilder();
                var sigJob = new SignatureJob(oldStream, builder);
                sigJob.Run();

                oldStream.Seek(0, SeekOrigin.Begin);

                var writer = new HumanReadableDeltaWriter();
                var deltaJob = new DeltaJob(builder.GetSignature(), newStream, writer);
                deltaJob.Run();

                return writer.ToString();
            }
        }

        [TestCase(
            "changes.input.01.in", "changes.input.01.in",
            "HEADER",
            "COPY 0 4818",
            "END")]
        [TestCase(
            "changes.input.02.in", "changes.input.01.in",
            "HEADER",
            "LITERAL 113",
            "COPY 0 4705",
            "END")]
        public void SanityTest(string oldFile, string newFile,
                               params string[] expected)
        {
            var delta = GetHumanReadableDelta(oldFile, newFile);

            Console.WriteLine(delta);

            Assert.AreEqual(Multiline(expected), delta);
        }
    }
}
