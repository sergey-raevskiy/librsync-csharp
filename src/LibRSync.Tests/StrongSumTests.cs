using System.Text;
using LibRSync.Core;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class StrongSumTests
    {
        [Test]
        public void NullTest()
        {
            var sum1 = new StrongSum();
            var sum2 = new StrongSum();

            Assert.IsTrue(sum1.Equals(sum2));
            Assert.IsTrue(sum2.Equals(sum1));

            Assert.AreEqual(0, sum1.GetHashCode());
            Assert.AreEqual(0, sum2.GetHashCode());
        }

        [TestCase("Hello world!")]
        [TestCase("Hash engine")]
        [TestCase("Umläut")]
        [TestCase("Lol")]
        public void EqualityTest(string input)
        {
            var buf1 = Encoding.UTF8.GetBytes(input);
            var buf2 = Encoding.UTF8.GetBytes(input);

            var sum1 = StrongSumAlgorithm.Md4.GetSum(buf1, 0, buf1.Length);
            var sum2 = StrongSumAlgorithm.Md4.GetSum(buf2, 0, buf2.Length);

            Assert.IsTrue(sum1.Equals(sum2));
            Assert.IsTrue(sum2.Equals(sum1));

            Assert.AreEqual(sum1.GetHashCode(), sum2.GetHashCode());
        }

        [TestCase("Hello world!", "Hello world?")]
        [TestCase("Hash engine", "HASH ENGINE")]
        public void NonEqualityTest(string input1, string input2)
        {
            var buf1 = Encoding.UTF8.GetBytes(input1);
            var buf2 = Encoding.UTF8.GetBytes(input2);

            var sum1 = StrongSumAlgorithm.Md4.GetSum(buf1, 0, buf1.Length);
            var sum2 = StrongSumAlgorithm.Md4.GetSum(buf2, 0, buf2.Length);

            Assert.IsFalse(sum1.Equals(sum2));
            Assert.IsFalse(sum2.Equals(sum1));
        }
    }
}
