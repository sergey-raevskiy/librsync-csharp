using LibRSync.Core;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class HashAlgorithmTest
    {
        [Test]
        public void GetAlgorithTest()
        {
            Assert.AreSame(StrongSumAlgorithm.Md4, StrongSumAlgorithm.GetAlgorithm("md4"));
            Assert.IsNull(StrongSumAlgorithm.GetAlgorithm("super-srypto"));
        }
    }
}
