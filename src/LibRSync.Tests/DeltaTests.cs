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
            var rdiff = new RDiff();

            using (var old = GetTestDataStream(oldName))
            using (var @new = GetTestDataStream(newName))
            using (var signature = new MemoryStream())
            using (var delta = new MemoryStream())
            using (var actual = new MemoryStream())
            {
                rdiff.GetSignature(old, signature);

                signature.Seek(0, SeekOrigin.Begin);
                rdiff.GetDelta(signature, @new, delta);

                delta.Seek(0, SeekOrigin.Begin);
                old.Seek(0, SeekOrigin.Begin);
                rdiff.Patch(old, delta, actual);

                actual.Seek(0, SeekOrigin.Begin);
                @new.Seek(0, SeekOrigin.Begin);

                Assert.AreEqual(StreamToArray(@new), StreamToArray(actual));
            }
        }
    }
}
