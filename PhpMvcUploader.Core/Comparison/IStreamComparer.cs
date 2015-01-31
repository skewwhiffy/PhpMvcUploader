using System.IO;

namespace PhpMvcUploader.Core.Comparison
{
    public interface IStreamComparer
    {
        bool BinaryCompare(Stream left, Stream right);
        bool TextCompare(Stream left, Stream right);
    }
}
