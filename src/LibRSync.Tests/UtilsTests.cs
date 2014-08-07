using LibRSync.Core;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        [TestCase(1L, 1)]
        [TestCase(0xffff, 2)]
        [TestCase(0x01ff, 2)]
        [TestCase(0xffffff, 4)]
        [TestCase(0x01ffff, 4)]
        [TestCase(0x1ffffff, 4)]
        [TestCase(0xffffabcd, 4)]
        [TestCase(0x1ffffabcd, 8)]
        [TestCase(0x7BCDabcdDEADBEEFL, 8)]
        [TestCase(0x7BCDabcd00000000L, 8)]
        public void IntLenTest(long i, int expected)
        {
            Assert.AreEqual(expected, Utils.GetIntLen(i));
        }
    }
}
