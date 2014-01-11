using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

            foreach (var len in lens)
                Emit(OpKind.Literal, 0, len);

            foreach (var len1 in lens)
                foreach (var len2 in lens)
                    Emit(OpKind.Literal, 0, len1, len2);

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

    internal class Patch : Job
    {
        private const int RS_DELTA_MAGIC = 0x72730236;

        private readonly Stream @base;
        private readonly Stream delta;
        private readonly Stream @new;

        private Opcode cmd;
        private long param1;
        private long param2;

        public Patch(Stream @base, Stream delta, Stream @new) : base("patch")
        {
            this.@base = @base;
            this.delta = delta;
            this.@new = @new;
        }

        protected override StateFunc InitialState()
        {
            return Header;
        }

        protected StateFunc Params()
        {
            param1 = NetInt.ReadInt(delta, cmd.Len1);
            param2 = NetInt.ReadInt(delta, cmd.Len2);

            return PatchRun;
        }

        private StateFunc Literal()
        {
            var buf = new byte[param1];
            delta.Read(buf, 0, buf.Length);
            @new.Write(buf, 0, buf.Length);

            return CommandByte;
        }

        private StateFunc Copy()
        {
            @base.Seek(param1, SeekOrigin.Begin);
            var buf = new byte[param2];
            @base.Read(buf, 0, buf.Length);
            @new.Write(buf, 0, buf.Length);

            return CommandByte;
        }

        private StateFunc PatchRun()
        {
            if (cmd.Kind == OpKind.Literal)
                return Literal;
            else if (cmd.Kind == OpKind.Copy)
                return Copy;
            else if (cmd.Kind == OpKind.End)
                return Completed;
            else
                throw new FileFormatException();
        }

        private StateFunc CommandByte()
        {
            var b = delta.ReadByte();
            if (b == -1)
                return Completed;

            cmd = OpcodePrototypes.Get((byte) b);

            if (cmd.Len1 > 0)
            {
                return Params;
            }
            else
            {
                param1 = cmd.Immediate;
                param2 = 0;

                return PatchRun;
            }
        }

        private StateFunc Header()
        {
            var signature = NetInt.ReadInt(delta);

            if (signature != RS_DELTA_MAGIC)
                throw new Exception();

            return CommandByte;
        }
    }
}
