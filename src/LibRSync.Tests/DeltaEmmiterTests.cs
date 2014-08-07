using System.IO;
using LibRSync.Core;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class DeltaEmmiterTests : TestBase
    {
        // ISSUE 2
        [Test]
        public void LiteralOpcodeTest()
        {
            using (var ms = new MemoryStream())
            {
                var emitter = new DeltaEmitter(ms);

                emitter.Header();
                emitter.Literal(new byte[0x1ffffff], 0, 0x1ffffff);
                emitter.Copy(0x1ffffff, 0x1ffffff);
                emitter.End();

                ms.Seek(0, SeekOrigin.Begin);

                var processor = new HumanReadableDeltaWriter();
                var readJob = new DeltaReadJob(ms, processor);
                readJob.Run();

                // 33554431 == 0x1ffffff
                Assert.AreEqual(Multiline(
                    "HEADER",
                    "LITERAL 33554431",
                    "COPY 33554431 33554431",
                    "END"),
                    processor.ToString());
            }
        }
    }
}
