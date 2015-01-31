using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhpMvcUploader.Common;

namespace PhpMvcUploader.Test.Helpers.TestData
{
    public class FileMaker
    {
        private readonly string _start;
        private readonly bool _absolutePaths;
        private readonly int _depth;
        private const int IoRetry = 15;
        private static readonly TimeSpan IoRetryInterval = TimeSpan.FromMilliseconds(100);

        private const int MaxFolderDepth = 3;
        private const int MaxFilesPerFolder = 4;
        private const int MaxFoldersPerFolder = 5;

        private readonly List<string> _files;
        private readonly List<string> _folders;
        private readonly Generator _generator;

        private FileMaker(string start, bool absolutePaths, int depth)
        {
            _start = start;
            _absolutePaths = absolutePaths;
            _depth = depth;
            _generator = new Generator();
            _folders = new List<string>();
            _files = new List<string>();
            MakeFolderTree();
        }

        public static FileMaker RelativePaths(string start, int depth = MaxFolderDepth)
        {
            return new FileMaker(start, false, depth);
        }

        public static FileMaker AbsolutePaths(string start, int depth = MaxFolderDepth)
        {
            return new FileMaker(start, true, depth);
        }

        private void MakeFolderTree(string start = null, int? depth = null)
        {
            if (depth == null)
            {
                depth = _depth;
            }
            if (start == null)
            {
                start = _start;
            }
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
                var newFolderRelative = _generator.GetUniqueString();
                var newFolder = Path.Combine(start, newFolderRelative);
                _folders.Add(newFolder);
                Directory.CreateDirectory(newFolder);
                MakeFolderTree(newFolder, depth - 1);
            }
        }

        private string GetRelative(string absolute)
        {
            if (!absolute.StartsWith(_start))
            {
                throw new InvalidOperationException("Something's gone wrong: '{0}' is not in '{1}'"
                    .FormatX(absolute, _start));
            }
            return absolute.Substring(_start.Length).Trim('\\', '/');
        }

        public bool DeleteFolder(string folder, int ioRetry = IoRetry, TimeSpan? ioRetryInterval = null)
        {
            if (!_absolutePaths)
            {
                folder = Path.Combine(_start, folder);
            }
            return DeleteFolderAbsolute(folder, ioRetry, IoRetryInterval);
        }

        public static bool DeleteFolderAbsolute(
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
            if (!_absolutePaths)
            {
                file = Path.Combine(_start, file);
            }
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
            if (!_absolutePaths)
            {
                from = Path.Combine(_start, from);
                to = Path.Combine(_start, to);
            }
            CopyFolderAbsolute(from, to);
        }

        public static void CopyFolderAbsolute(string from, string to)
        {
            //Now Create all of the directories
            Directory.GetDirectories(from, "*", SearchOption.AllDirectories)
                .ForEach(dirPath => Directory.CreateDirectory(dirPath.Replace(from, to)));

            //Copy all the files & Replaces any files with the same name
            Directory.GetFiles(from, "*.*", SearchOption.AllDirectories)
                .ForEach(newPath => File.Copy(newPath, newPath.Replace(from, to), true));
        }

        public List<string> Files
        {
            get
            {
                if (_absolutePaths)
                {
                    return _files;
                }
                return _files.Select(GetRelative).ToList();
            }
        }

        public List<string> Folders
        {
            get
            {
                if (_absolutePaths)
                {
                    return _folders;
                }
                return _folders.Select(GetRelative).ToList();
            }
        } 
    }
}
