namespace PhpMvcUploader.Core.Io
{
    public interface IFileSystemFactory
    {
        FileSystem Build(string root);
    }
}