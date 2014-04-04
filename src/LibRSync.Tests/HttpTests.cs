using System;
using System.IO;
using System.Net.Http;
using Nancy;
using Nancy.Hosting.Self;
using NUnit.Framework;

namespace LibRSync.Tests
{
    [TestFixture]
    public class HttpTests : TestBase
    {
        private NancyHost host;
        public HttpClient Client { get; set; }

        private class PathProvider : IRootPathProvider
        {
            private string path;

            public PathProvider(string path)
            {
                this.path = path;
            }

            public string GetRootPath()
            {
                return path;
            }
        }

        private class TestBootstrapper : DefaultNancyBootstrapper
        {
            private PathProvider provider;

            public TestBootstrapper(string rootPath)
            {
                this.provider = new PathProvider(rootPath);
            }

            protected override IRootPathProvider RootPathProvider
            {
                get { return provider; }
            }
        }

        private string oldDir;

        [SetUp]
        public void SetUp()
        {
            oldDir = Directory.GetCurrentDirectory();
            var newDir = Path.Combine(oldDir, TestContext.CurrentContext.Test.FullName.Replace("\"", ""));

            if (Directory.Exists(newDir))
            {
                Directory.Delete(newDir, true);
            }

            Directory.CreateDirectory(newDir);
            Directory.SetCurrentDirectory(newDir);

            var cfg = new HostConfiguration
            {
                UrlReservations = new UrlReservations
                {
                    CreateAutomatically = true,
                    User = "Everyone"
                }
            };

            var baseAddress = new Uri("http://localhost:12345");

            host = new NancyHost(new TestBootstrapper(newDir), cfg, baseAddress);
            host.Start();

            Client = new HttpClient
            {
                BaseAddress = baseAddress
            };
        }

        [TearDown]
        public void TearDown()
        {
            Client.Dispose();
            host.Dispose();

            Directory.SetCurrentDirectory(oldDir);
        }

        private static readonly HttpMethod Patch = new HttpMethod("PATCH");

        [TestCase("delta.input.01.delta", "delta.input.01.expect")]
        [TestCase("delta.input.02.delta", "delta.input.02.expect")]
        [TestCase("delta.input.03.delta", "delta.input.03.expect")]
        public void PatchTest(string deltaName, string expectedName)
        {
            File.WriteAllBytes("file.txt", new byte[0]);

            var request = new HttpRequestMessage(Patch, "/res/file.txt");
            request.Content = new StreamContent(GetTestDataStream(deltaName));

            var res = Client.SendAsync(request).Result;
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, res.StatusCode);

            Assert.AreEqual(
                StreamToArray(GetTestDataStream(expectedName)),
                File.ReadAllBytes("file.txt"));
        }
    }
}
