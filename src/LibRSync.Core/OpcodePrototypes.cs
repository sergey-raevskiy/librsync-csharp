using System.Collections.Generic;
using System.Diagnostics;

namespace LibRSync.Core
{
    internal class OpcodePrototypes
    {
        private static List<Opcode> prototypes = new List<Opcode>();

        private static void Emit(OpKind kind, int immediate = 0, int len1 = 0, int len2 = 0)
        {
            prototypes.Add(new Opcode(kind, immediate, len1, len2));
        }

        static OpcodePrototypes()
        {
            var lens = new[] {1, 2, 4, 8};

            Emit(OpKind.End);

            for (var i = 1; i <= 64; i++)
                Emit(OpKind.Literal, i);

            Debug.Assert(prototypes.Count == 65);

            foreach (var len in lens)
                Emit(OpKind.Literal, 0, len);

            Debug.Assert(prototypes.Count == 69);

            foreach (var len1 in lens)
                foreach (var len2 in lens)
                    Emit(OpKind.Copy, 0, len1, len2);

            Debug.Assert(prototypes.Count == 85);

            while (prototypes.Count < 256)
                Emit(OpKind.Reserved, prototypes.Count);

            Debug.Assert(prototypes.Count == 256);
        }

        public static Opcode Get(byte op)
        {
            return prototypes[op];
        }
    }
}
