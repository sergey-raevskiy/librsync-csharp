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
            Assert.AreSame(StrongHashAlgorithm.Md4, StrongHashAlgorithm.GetAlgorithm("md4"));
            Assert.IsNull(StrongHashAlgorithm.GetAlgorithm("super-srypto"));
        }
    }
}
