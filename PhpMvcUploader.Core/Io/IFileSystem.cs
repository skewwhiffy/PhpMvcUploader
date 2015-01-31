using System.Collections.Generic;
using System.IO;

namespace PhpMvcUploader.Core.Io
{
    public interface IFileSystem
    {
        IEnumerable<string> GetDirectoriesRecursive(string path = "");
        IEnumerable<string> GetFilesRecursive(string path = "");
        IEnumerable<string> GetFiles(string path = "");
        string AbsolutePath(string relativePath);
        Stream OpenRead(string path);
    }
}
