using System.Collections.Generic;
using System.Linq;
using PhpMvcUploader.Common;
using PhpMvcUploader.Core.Io;

namespace PhpMvcUploader.Core.Comparison
{
    public class FolderComparer
    {
        private readonly StreamComparer _comparer;
        private readonly IFileSystem _before;
        private readonly IFileSystem _after;

        private List<string> _different;
        private List<string> _deletedFiles;
        private List<string> _deletedFolders;
        private List<string> _newFiles;
        private List<string> _newFolders;
        private List<string> _commonFolders;

        private bool _compared;

        public FolderComparer(
            IFileSystemFactory fileSystemFactory,
            StreamComparer comparer,
            string before,
            string after)
        {
            _comparer = comparer;
            _before = fileSystemFactory.Build(before);
            _after = fileSystemFactory.Build(after);
        }

        private void Compare()
        {
            CompareFolders();
            CompareFiles();
            _compared = true;
        }

        private void CompareFiles()
        {
            _newFiles = _newFolders
                .SelectMany(_after.GetFilesRecursive)
                .ToList();
            _different = new List<string>();
            _commonFolders.ForEach(CompareFilesInFolder);
            _deletedFiles = new List<string>();
        }

        private void CompareFilesInFolder(string d)
        {
            var beforeFiles = _before.GetFiles(d);
            var afterFiles = _after.GetFiles(d).ToList();
            _newFiles.AddRange(afterFiles.Where(f => !beforeFiles.Contains(f)));
            beforeFiles.Intersect(afterFiles).ForEach(CompareFile);
        }

        private void CompareFile(string file)
        {
            using (var beforeFile = _before.OpenRead(file))
            using (var afterFile = _after.OpenRead(file))
            {
                if (!_comparer.BinaryCompare(beforeFile, afterFile))
                {
                    _different.Add(file);
                }
            }
        }

        private void CompareFolders()
        {
            var beforeDirectories = _before.GetDirectoriesRecursive().ToList();
            var afterDirectories = _after.GetDirectoriesRecursive().ToList();
            _newFolders = afterDirectories.FindAll(d => !beforeDirectories.Contains(d));
            _deletedFolders = beforeDirectories.FindAll(d => !afterDirectories.Contains(d));
            _commonFolders = afterDirectories.Intersect(beforeDirectories).ToList();
        }

        public List<string> Different
        {
            get
            {
                if (!_compared)
                {
                    Compare();
                }
                return _different;
            }
        }

        public List<string> DeletedFiles
        {
            get
            {
                if (!_compared)
                {
                    Compare();
                }
                return _deletedFiles;
            }
        }

        public List<string> DeletedFolders
        {
            get
            {
                if (!_compared)
                {
                    Compare();
                }
                return _deletedFolders;
            }
        }

        public List<string> NewFiles
        {
            get
            {
                if (!_compared)
                {
                    Compare();
                }
                return _newFiles;
            }
        }

        public List<string> NewFolders
        {
            get
            {
                if (!_compared)
                {
                    Compare();
                }
                return _newFolders;
            }
        } 
    }
}
