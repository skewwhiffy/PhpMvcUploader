using System.Collections.Generic;

namespace PhpMvcUploader.Core.Comparison
{
    public class FileComparer
    {
        public static readonly IList<string> TextExtensions = new List<string>
        {
            "txt",
            "htm",
            "html",
            "php"
        }.AsReadOnly();

        public FileComparer(IStreamComparer streamComparer)
        {
            
        }
    }
}
