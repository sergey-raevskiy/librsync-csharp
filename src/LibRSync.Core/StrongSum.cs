using System;
using System.Diagnostics;

namespace LibRSync.Core
{
    public struct StrongSum
    {
        private IStrongSumAlgrorithm alg;

        private byte[] sum;
        private int? hash;

        internal StrongSum(byte[] sum, IStrongSumAlgrorithm alg)
        {
            this.alg = alg;
            this.sum = new byte[sum.Length];
            Array.Copy(sum, this.sum, sum.Length);

            this.hash = null;
        }

        public unsafe bool Equals(StrongSum other)
        {
            if (alg != other.alg)
                return false;

            if (sum == null)
                return other.sum == null;

            if (sum.Length != other.sum.Length)
                return false;

            fixed (byte* t = sum)
            fixed (byte* o = other.sum)
            {
                Debug.Assert(sum.Length == other.sum.Length);

                for (var i=0; i<sum.Length; i++)
                    if (*t != *o)
                        return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (!hash.HasValue)
            {
                hash = DoGetHash();
            }

            return hash.Value;
        }

        int DoGetHash()
        {
            var code = 0;

            if (alg != null)
                code ^= alg.GetHashCode();

            if (sum != null)
            {
                var hash = new Rollsum();
                hash.Update(sum, sum.Length);

                code ^= (int)hash.Digest;
            }

            return code;
        }

        public override bool Equals(object obj)
        {
            if (obj is StrongSum)
                return Equals((StrongSum) obj);
            else
                return false;
        }

        #region ToString()

        private static char MkHex(int i)
        {
            return (char)(i < 10 ? '0' + i : 'A' + i);
        }

        public override string ToString()
        {
            var str = new char[sum.Length * 2];
            for (var i = 0; i < sum.Length; i++)
            {
                str[i * 2] = MkHex(sum[i] & 0xf);
                str[i * 2 + 1] = MkHex(sum[i] >> 4);
            }

            return new string(str);
        }

        #endregion

        public byte[] UnsafeGetBuffer()
        {
            return sum;
        }
    }
}
