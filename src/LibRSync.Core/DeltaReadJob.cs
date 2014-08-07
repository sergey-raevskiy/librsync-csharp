using System;
using System.IO;

namespace LibRSync.Core
{
    public class DeltaReadJob : Job
    {
        private readonly Stream delta;
        private readonly IDeltaProcessor processor;

        private Opcode cmd;
        private long param1;
        private long param2;

        public DeltaReadJob(Stream delta,IDeltaProcessor processor)
            : base("read-delta")
        {
            this.delta = delta;
            this.processor = processor;
        }

        protected override StateFunc InitialState()
        {
            return Header;
        }

        protected StateFunc Params()
        {
            NetInt.ReadInt(delta, cmd.Len1, out param1);
            NetInt.ReadInt(delta, cmd.Len2, out param2);

            return PatchRun;
        }

        private StateFunc Literal()
        {
            var buf = new byte[param1];
            delta.Read(buf, 0, buf.Length);
            processor.Literal(buf, 0, buf.Length);

            return CommandByte;
        }

        private StateFunc Copy()
        {
            processor.Copy(param1, param2);

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
                throw new FileFormatException("Unexpected end of file");

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

            if (signature != Const.RS_DELTA_MAGIC)
                throw new Exception();

            return CommandByte;
        }
    }
}
