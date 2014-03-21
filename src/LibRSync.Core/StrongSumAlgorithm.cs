using System.Collections.Generic;
using LibRSync.Core.Compat;

namespace LibRSync.Core
{
    public class StrongSumAlgorithm
    {
        public static readonly IStrongSumAlgrorithm Md4 = new Md4SumAlgrorithm();

        private static Dictionary<string, IStrongSumAlgrorithm> algorithms
            = new Dictionary<string, IStrongSumAlgrorithm>
            {
                {"md4", Md4}
            };

        public static IStrongSumAlgrorithm GetAlgorithm(string name)
        {
            IStrongSumAlgrorithm alg;

            if (algorithms.TryGetValue(name, out alg))
                return alg;
            else
                return null;
        }
    }
}
