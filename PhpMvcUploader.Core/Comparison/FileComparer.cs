using System.IO;
using PhpMvcUploader.Common;

namespace PhpMvcUploader.Core.Comparison
{
    public class FileComparer
    {
        private const int BufferSize = 2048;

        public bool BinaryCompare(Stream left, Stream right)
        {
            var leftBuffer = new byte[BufferSize];
            var rightBuffer = new byte[BufferSize];
            while (true)
            {
                var leftRead = left.Read(leftBuffer, 0, BufferSize);
                var rightRead = right.Read(rightBuffer, 0, BufferSize);
                if (leftRead != rightRead)
                {
                    return false;
                }
                if (leftRead == 0)
                {
                    return true;
                }
                for (var i = 0; i < leftRead; i++)
                {
                    if (leftBuffer[i] != rightBuffer[i])
                    {
                        return false;
                    }
                }
            }
        }

        public bool TextCompare(Stream left, Stream right)
        {
            var leftReader = new StreamReader(left);
            var rightReader = new StreamReader(right);
            while (true)
            {
                if (leftReader.EndOfStream && rightReader.EndOfStream)
                {
                    return true;
                }
                var leftRead = GetNextNonEmptyLine(leftReader);
                var rightRead = GetNextNonEmptyLine(rightReader);
                if (leftRead != rightRead)
                {
                    return false;
                }
            }
        }

        private string GetNextNonEmptyLine(TextReader reader)
        {
            while (true)
            {
                var read = reader.ReadLine();
                if (read == null)
                {
                    return null;
                }
                read = read.Trim('\0', '\n', '\r');
                if (!read.IsNullOrWhiteSpace())
                {
                    return read;
                }
            }
        }
    }
}
