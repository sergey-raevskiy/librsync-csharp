namespace LibRSync.Core
{
    struct Opcode
    {
        public readonly OpKind Kind;
        public readonly int Immediate;
        public readonly int Len1;
        public readonly int Len2;

        public Opcode(OpKind kind,
            int immediate,
            int len1,
            int len2)
        {
            Kind = kind;
            Immediate = immediate;
            Len1 = len1;
            Len2 = len2;
        }
    }
}
