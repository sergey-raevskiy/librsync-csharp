using System.IO;
using LibRSync.Core;

namespace LibRSync.Tests
{
    class HumanReadableDeltaWriter : IDeltaProcessor
    {
        private StringWriter writer = new StringWriter();

        public void Header()
        {
            writer.WriteLine("HEADER");
        }

        public void Copy(long start, long length)
        {
            writer.WriteLine("COPY {0} {1}", start, length);
        }

        public void Literal(byte[] data, long offset, long count)
        {
            writer.WriteLine("LITERAL {0}", count);
        }

        public void End()
        {
            writer.WriteLine("END");
        }

        public override string ToString()
        {
            return writer.ToString();
        }
    }
}
