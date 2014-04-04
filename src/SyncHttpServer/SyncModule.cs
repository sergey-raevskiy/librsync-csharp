using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using LibRSync.Core;
using Nancy;

namespace SyncHttpServer
{
    public class SyncModule : NancyModule
    {
        private readonly IRootPathProvider pathProvider;

        public SyncModule(IRootPathProvider pathProvider)
        {
            this.pathProvider = pathProvider;

            Patch["/res/{path*}"] = PatchFile;
        }

        private FileStream OpenTempFile(string filePath, out string tempPath)
        {
            var dir = Path.GetDirectoryName(filePath);
            tempPath = Path.Combine(dir, Guid.NewGuid().ToString("n"));

            return File.Open(tempPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
        }

        private string GetETag(Stream stream)
        {
            using (var md5 = new MD5Cng())
            {
                md5.ComputeHash(stream);
                return Convert.ToBase64String(md5.Hash);
            }
        }

        [Flags]
        internal enum MoveFileFlags
        {
            None = 0,
            ReplaceExisting = 1,
            CopyAllowed = 2,
            DelayUntilReboot = 4,
            WriteThrough = 8,
            CreateHardlink = 16,
            FailIfNotTrackable = 32,
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool MoveFileEx(
            string lpExistingFileName,
            string lpNewFileName,
            MoveFileFlags dwFlags);

        private object PatchFile(dynamic p)
        {
            var path = Path.Combine(pathProvider.GetRootPath(), p.path);
            string tempPath;
            string etag;

            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Delete))
            {
                using (var tempFile = OpenTempFile(path, out tempPath))
                {
                    var patchJob = new PatchJob(file, Request.Body, tempFile);
                    patchJob.Run();
                }

                using (var tempFile = File.Open(tempPath, FileMode.Open, FileAccess.Read))
                {
                    etag = GetETag(tempFile);
                }
            }

            if (!MoveFileEx(tempPath, path, MoveFileFlags.ReplaceExisting))
                throw new Win32Exception();

            var response = new Response();
            response.StatusCode = HttpStatusCode.NoContent;
            response.Headers.Add("ETag", "\"" + etag + "\"");

            return response;
        }
    }
}
