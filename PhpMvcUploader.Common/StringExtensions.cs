using System.Collections.Generic;

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
    }
}
