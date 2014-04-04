using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
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

            return File.Open(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
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

            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Delete))
            {
                using (var tempFile = OpenTempFile(path, out tempPath))
                {
                    var patchJob = new PatchJob(file, Request.Body, tempFile);
                    patchJob.Run();
                }
            }

            if (!MoveFileEx(tempPath, path, MoveFileFlags.ReplaceExisting))
                throw new Win32Exception();

            return 204;
        }
    }
}
