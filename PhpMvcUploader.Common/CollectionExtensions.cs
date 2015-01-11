using System;
using System.Collections.Generic;

namespace PhpMvcUploader.Common
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var s in source)
            {
                action(s);
            }
        }
    }
}
