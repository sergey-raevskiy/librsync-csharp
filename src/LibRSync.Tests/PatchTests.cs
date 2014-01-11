using System.IO;
using LibRSync.Core;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class PatchTests : TestBase
    {
        [TestCase("delta.input.01.delta", "delta.input.01.expect")]
        [TestCase("delta.input.02.delta", "delta.input.02.expect")]
        [TestCase("delta.input.03.delta", "delta.input.03.expect")]
        public void BasicTest(string deltaName, string expectedName)
        {
            var delta = GetTestDataStream(deltaName);
            var expected = StreamToArray(GetTestDataStream(expectedName));

            var rdiff = new RDiff();

            using (var ms = new MemoryStream())
            {
                rdiff.Patch(new MemoryStream(), delta, ms);
                var actual = ms.ToArray();

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
