using System;
using System.Collections.Generic;
using System.IO;
using PhpMvcUploader.Common;

namespace PhpMvcUploader.Test.Helpers.TestData
{
    public class FileMaker
    {
        private const int IoRetry = 15;
        private static readonly TimeSpan IoRetryInterval = TimeSpan.FromMilliseconds(100);

        private const int MaxFolderDepth = 5;
        private const int MaxFilesPerFolder = 4;
        private const int MaxFoldersPerFolder = 3;

        private readonly List<string> _files;
        private readonly List<string> _folders;
        private readonly Generator _generator;

        public FileMaker()
        {
            _generator = new Generator();
            _folders = new List<string>();
            _files = new List<string>();
        }

        public void MakeFolderTree(string start, int depth = MaxFolderDepth)
        {
            var numberOfFiles = _generator.RandomIntInclusive(1, MaxFilesPerFolder);
            for (var i = 0; i < numberOfFiles; i++)
            {
                var newFileName = _generator.GetUniqueString();
                var newFilePath = Path.Combine(start, "{0}.txt".FormatX(newFileName));
                var newFileContents = "This is file {0}{1}"
                    .FormatX(newFilePath, Environment.NewLine)
                    .GetBytes();
                _files.Add(newFilePath);
                using (var stream = File.Create(newFilePath))
                {
                    stream.Write(newFileContents, 0, newFileContents.Length);
                    stream.Close();
                }
            }
            if (depth == 0)
            {
                return;
            }
            var numberOfFolders = _generator.RandomIntInclusive(1, MaxFoldersPerFolder);
            for (var i = 0; i < numberOfFolders; i++)
            {
                var newFolder = Path.Combine(start, _generator.GetUniqueString());
                _folders.Add(newFolder);
                Directory.CreateDirectory(newFolder);
                MakeFolderTree(newFolder, depth - 1);
            }
        }

        public bool DeleteFolder(
            string folder,
            int ioRetry = IoRetry,
            TimeSpan? ioRetryInterval = null)
        {
            if (!ioRetryInterval.HasValue)
            {
                ioRetryInterval = IoRetryInterval;
            }
            try
            {
                ioRetry.TimesEvery(ioRetryInterval.Value)
                    .Try(() => Directory.Delete(folder, true));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteFile(
            string file,
            int ioRetry = IoRetry,
            TimeSpan? ioRetryInterval = null)
        {
            if (!ioRetryInterval.HasValue)
            {
                ioRetryInterval = IoRetryInterval;
            }
            try
            {
                ioRetry.TimesEvery(ioRetryInterval.Value)
                    .Try(() => File.Delete(file));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CopyFolder(string from, string to)
        {
            //Now Create all of the directories
            Directory.GetDirectories(from, "*", SearchOption.AllDirectories)
                .ForEach(dirPath => Directory.CreateDirectory(dirPath.Replace(from, to)));

            //Copy all the files & Replaces any files with the same name
            Directory.GetFiles(from, "*.*", SearchOption.AllDirectories)
                .ForEach(newPath => File.Copy(newPath, newPath.Replace(from, to), true));
        }

        public List<string> Files { get { return _files; } }

        public List<string> Folders { get { return _folders; } } 
    }
}
