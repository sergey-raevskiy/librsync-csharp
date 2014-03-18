using System;
using System.IO;

namespace LibRSync.Core
{
    public class DeltaJob : Job
    {
        private Signature signature;
        private IDeltaProcessor processor;

        private Stream @new;

        private byte[] chunk;
        private int chunkLen;
        private Rollsum rs;

        public DeltaJob(Signature signature,
                        Stream @new,
                        IDeltaProcessor processor)
            : base("delta")
        {
            this.signature = signature;
            this.processor = processor;
            this.@new = @new;
        }

        protected override StateFunc InitialState()
        {
            return ReadChunk;
        }

        private StateFunc ReadChunk()
        {
            chunk = new byte[signature.ChunkSize];
            chunkLen = @new.Read(chunk, 0, chunk.Length);

            if (chunkLen == 0)
                return Flush; // ???

            rs = new Rollsum();
            rs.Update(chunk, chunkLen);

            return Search;
        }

        private StateFunc Flush()
        {
            return Completed;
        }

        private StateFunc Search()
        {
            var weak = rs.Digest;
            byte[] strong = null;

            if (signature.HasWeak(weak))
            {
                if (strong == null)
                {
                    strong = Checksum.Strong(chunk, chunkLen);
                }

                var block = signature.LookupBlock(weak, strong);
                if (block != null)
                {
                    processor.Copy(block.Start, chunkLen);
                    return ReadChunk;
                }
            }

            return Rotate;
        }

        private StateFunc Rotate()
        {
            var o = chunk[0];
            var i = @new.ReadByte();

            if (i == -1)
                return Flush;

            rs.Rotate((byte) i, o);

            Array.Copy(chunk, 1, chunk, 0, chunkLen - 1);
            chunk[chunkLen - 1] = (byte) i;

            processor.Literal(new [] {o}, 0, 1);

            return Search;
        }
    }
}
