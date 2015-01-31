using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhpMvcUploader.Core.Io
{
    public class FileSystem : IFileSystem
    {
        private readonly string _root;

        public FileSystem(string root)
        {
            _root = Path.GetFullPath(TrimSlashes(root));
        }

        public IEnumerable<string> GetDirectoriesRecursive(string path = "")
        {
            path = TrimSlashes(path);
            return Directory
                .GetDirectories(Path.Combine(_root, path), "*", SearchOption.AllDirectories)
                .Select(d => d.Substring(_root.Length))
                .Select(TrimSlashes);
        } 

        public IEnumerable<string> GetFilesRecursive(string path = "")
        {
            path = TrimSlashes(path);
            return Directory
                .GetFiles(Path.Combine(_root, path), "*", SearchOption.AllDirectories)
                .Select(f => f.Substring(_root.Length))
                .Select(TrimSlashes);
        }

        public IEnumerable<string> GetFiles(string path = "")
        {
            path = TrimSlashes(path);
            return Directory
                .GetFiles(Path.Combine(_root, path), "*", SearchOption.TopDirectoryOnly)
                .Select(f => f.Substring(_root.Length))
                .Select(TrimSlashes);
        }

        public string AbsolutePath(string relativePath)
        {
            relativePath = TrimSlashes(relativePath);
            return Path.Combine(_root, relativePath);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(Path.Combine(_root, TrimSlashes(path)));
        }

        private string TrimSlashes(string source)
        {
            return source.Trim('\\', '/');
        }
    }
}
