using System.IO;

namespace LibRSync.Tests
{
    public class TestBase
    {
        protected Stream GetTestDataStream(string name)
        {
            return GetType().Assembly.GetManifestResourceStream(GetType(), "TestData." + name);
        }

        protected byte[] StreamToArray(Stream s)
        {
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();
            }
        }

        protected static string Multiline(params string[] lines)
        {
            var writer = new StringWriter();
            foreach (var line in lines)
                writer.WriteLine(line);

            return writer.ToString();
        }
    }
}
