using System;
using System.Collections.Generic;

namespace PhpMvcUploader.Common
{
    public static class CollectionExtensions
    {
        private static Random _randomGenerator;

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var s in source)
            {
                action(s);
            }
        }

        public static T Random<T>(this IList<T> source)
        {
            return source[RandomGenerator.Next(source.Count)];
        }

        public static T[] Copy<T>(this T[] source)
        {
            var result = new T[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = source[i];
            }
            return result;
        }

        private static Random RandomGenerator
        {
            get { return _randomGenerator ?? (_randomGenerator = new Random()); }
        }
    }
}
