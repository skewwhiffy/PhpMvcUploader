using System.Collections.Generic;
using System.Text;

namespace PhpMvcUploader.Common
{
    public static class StringExtensions
    {
        public static string FormatX(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string JoinX(this IEnumerable<string> strings, string separator = ", ")
        {
            return string.Join(separator, strings);
        }

        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        public static byte[] GetBytes(this string source, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Unicode;
            }
            return encoding.GetBytes(source);
        }
    }
}
