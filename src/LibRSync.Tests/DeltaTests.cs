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

                var proc = new PatchProcessor(old2, actual);
                var deltaJob = new DeltaJob(builder.GetSignature(), @new, proc);
                deltaJob.Run();

                actual.Seek(0, SeekOrigin.Begin);
                @new.Seek(0, SeekOrigin.Begin);

                Assert.AreEqual(StreamToArray(@new), StreamToArray(actual));
            }
        }
    }
}
