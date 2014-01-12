using System;
using LibRSync.Core;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class RollsumTests
    {
        [TestCase(1337)]
        [TestCase(0)]
        [TestCase(34)]
        public void WeakVsRollsum(int seed)
        {
            var r = new Random(seed);
            var b = new byte[r.Next(1 << 16)];
            r.NextBytes(b);

            var rs = new Rollsum();
            rs.Update(b, b.Length);
            Assert.AreEqual(rs.Digest, Checksum.Weak(b, b.Length));
        }
    }
}
