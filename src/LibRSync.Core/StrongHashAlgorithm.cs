using System.Collections.Generic;
using LibRSync.Core.Compat;

namespace LibRSync.Core
{
    public class StrongHashAlgorithm
    {
        public static readonly IStrongHashAlgorithm Md4 = new Md4HashAlgorithm();

        private static Dictionary<string, IStrongHashAlgorithm> algorithms
            = new Dictionary<string, IStrongHashAlgorithm>
            {
                {"md4", Md4}
            };

        public static IStrongHashAlgorithm GetAlgorithm(string name)
        {
            IStrongHashAlgorithm alg;

            if (algorithms.TryGetValue(name, out alg))
                return alg;
            else
                return null;
        }
    }
}
