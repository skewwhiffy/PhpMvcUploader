using System;
using System.Collections;
using System.Collections.Generic;
using PhpMvcUploader.Common;

namespace PhpMvcUploader.Test.Helpers.TestData
{
    public class Generator
    {
        private static string _nextUniqueString = "a";
        private static readonly Random Random = new Random();

        public string GetUniqueString()
        {
            var uniqueString = _nextUniqueString;
            _nextUniqueString = AdvanceUniqueString(_nextUniqueString);
            return uniqueString;
        }

        public T RandomElement<T>(IList<T> source)
        {
            return source[RandomIntInclusive(source.Count - 1)];
        }

        public int RandomIntInclusive(int max)
        {
            return RandomIntInclusive(0, max);
        }

        public int RandomIntInclusive(int min, int max)
        {
            return Random.Next(min, max + 1);
        }

        private string AdvanceUniqueString(string current)
        {
            if (current.IsNullOrEmpty())
            {
                return "a";
            }
            var lastCharacter = current[current.Length - 1];
            var allButLastCharacter = current.Substring(0, current.Length - 1);
            if (lastCharacter >= 'a' && lastCharacter < 'z')
            {
                return "{0}{1}".FormatX(allButLastCharacter, (char)(lastCharacter + 1));
            }
            return AdvanceUniqueString(allButLastCharacter) + 'a';
        }
    }
}
