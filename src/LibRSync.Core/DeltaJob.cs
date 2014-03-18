using System;
using System.Collections.Generic;
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

        private List<byte> miss = new List<byte>();

        private long copyBase = 0;
        private long copyLen = 0;

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
            if (copyLen != 0)
            {
                processor.Copy(copyBase, copyLen);
            }

            if (miss.Count != 0)
            {
                processor.Literal(miss.ToArray(), 0, miss.Count);
            }

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
                    if (miss.Count != 0)
                    {
                        processor.Literal(miss.ToArray(), 0, miss.Count);
                        miss.Clear();
                    }

                    AppendCopy(block.Start, chunkLen);

                    return ReadChunk;
                }
            }

            return Rotate;
        }

        private void AppendCopy(long start, int i)
        {
            if (copyBase + copyLen == start)
            {
                copyLen += i;
            }
            else
            {
                if (copyLen != 0)
                {
                    processor.Copy(copyBase, copyLen);
                }

                copyBase = start;
                copyLen = i;
            }
        }

        private StateFunc Rotate()
        {
            if (copyLen != 0)
            {
                processor.Copy(copyBase, copyLen);
                copyLen = 0;
            }

            if (chunkLen == 0)
            {
                return Flush;
            }

            var o = chunk[0];

            miss.Add(o);
            Array.Copy(chunk, 1, chunk, 0, chunkLen - 1);

            var i = @new.ReadByte();
            if (i != -1)
            {
                rs.Rotate((byte)i, o);
                chunk[chunkLen - 1] = (byte)i;
            }
            else
            {
                rs.Rollout(o);
                chunkLen--;
            }

            return Search;
        }
    }
}
