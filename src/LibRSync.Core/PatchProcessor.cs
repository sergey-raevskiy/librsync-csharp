using System;
using System.IO;

namespace LibRSync.Core
{
    public class PatchProcessor : IDeltaProcessor
    {
        private readonly Stream @base;
        private readonly Stream @new;

        public PatchProcessor(Stream @base, Stream @new)
        {
            this.@new = @new;
            this.@base = @base;
        }

        public void Header()
        {}

        public void Copy(long start, long length)
        {
            @base.Seek(start, SeekOrigin.Begin);
            var buf = new byte[length];
            @base.Read(buf, 0, buf.Length);
            @new.Write(buf, 0, buf.Length);
        }

        public void Literal(byte[] data, long offset, long count)
        {
            @new.Write(data, (int) offset, (int) count);
        }

        public void End()
        {}
    }
}
