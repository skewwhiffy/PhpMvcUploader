namespace PhpMvcUploader.Core.Io
{
    public class FileSystemFactory : IFileSystemFactory
    {
        public FileSystem Build(string root)
        {
            return new FileSystem(root);
        }
    }
}
