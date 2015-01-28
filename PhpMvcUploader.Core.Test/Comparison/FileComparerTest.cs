using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PhpMvcUploader.Common;
using PhpMvcUploader.Core.Comparison;
using PhpMvcUploader.Test.Helpers.TestData;

namespace PhpMvcUploader.Core.Test.Comparison
{
    [TestFixture]
    public class FileComparerTest
    {
        const int Length = 100000;

        private byte[] _sequence;
        private byte[] _identicalSequence;
        private byte[] _longerSequence;

        private MemoryStream _stream;
        private MemoryStream _identicalStream;
        private MemoryStream _longerStream;

        private List<string> _stringSequence;
        private List<string> _identicalStringSequence;
        private List<string> _longerStringSequence;

        private FileComparer _comparer;
        private Generator _generator;

        [SetUp]
        public void BeforeEach()
        {
            _generator = new Generator();
            _sequence = _generator.RandomByteArray(Length);
            _stream = new MemoryStream(_sequence);

            _identicalSequence = new byte[Length];
            _longerSequence = new byte[Length * 2];
            for (int i = 0; i < Length; i++)
            {
                _identicalSequence[i] = _sequence[i];
                _longerSequence[i] = _sequence[i];
                _longerSequence[i + Length] = _sequence[i];
            }
            _identicalStream = new MemoryStream(_identicalSequence);
            _longerStream = new MemoryStream(_longerSequence);
            _comparer = new FileComparer();

            _stringSequence = Enumerable
                .Range(0, Length)
                .Select(i => _generator.GetUniqueString())
                .ToList();
            _identicalStringSequence = _stringSequence.ToList();
            _longerStringSequence = _stringSequence.Union(_stringSequence).ToList();
        }

        [Test]
        public void BinaryCompareWorksForIdenticalStreams()
        {
            var result = _comparer.BinaryCompare(_stream, _identicalStream);

            Assert.That(result);
        }

        [Test]
        public void BinaryCompareWorksForDifferentLengthStreams()
        {
            var result = _comparer.BinaryCompare(_stream, _longerStream);

            Assert.That(result, Is.False);
        }

        [Test]
        public void BinaryCompareWorksForDifferentValues()
        {
            _identicalSequence[Length/2] ++;

            var result = _comparer.BinaryCompare(_stream, _identicalStream);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TextCompareWorksForIdenticalstreams()
        {
            var firstStream = WithWindowsEndings(_stringSequence);
            var secondStream = WithWindowsEndings(_identicalStringSequence);

            var result = _comparer.TextCompare(firstStream, secondStream);

            Assert.That(result);
        }

        [Test]
        public void TextCompareWorksForSingleCharacterDifference()
        {
            var firstStream = WithWindowsEndings(_stringSequence);
            var rowToChange = _generator.RandomIntExclusive(_identicalStringSequence.Count);
            _identicalStringSequence[rowToChange] = "*" + _identicalStringSequence;
            var secondStream = WithWindowsEndings(_identicalStringSequence);

            var result = _comparer.TextCompare(firstStream, secondStream);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TextCompareIgnoresLineEndingStyle()
        {
            var firstStream = WithWindowsEndings(_stringSequence);
            var secondStream = WithLinuxEndings(_identicalStringSequence);

            var result = _comparer.TextCompare(firstStream, secondStream);

            Assert.That(result);
        }

        [Test]
        public void TextCompareIgnoresEmptyLines()
        {
            var newStringSequence = _stringSequence
                .SelectMany(s => new[] {s, " "});
            var firstStream = WithLinuxEndings(_stringSequence);
            var secondStream = WithLinuxEndings(newStringSequence);

            var result = _comparer.TextCompare(firstStream, secondStream);

            Assert.That(result);
        }

        private Stream WithWindowsEndings(IEnumerable<string> source)
        {
            var sourceStream = source.JoinX("\r\n");
            return new MemoryStream(sourceStream.GetBytes());
        }

        private Stream WithLinuxEndings(IEnumerable<string> source)
        {
            var sourceStream = source.JoinX("\n");
            return new MemoryStream(sourceStream.GetBytes());
        }
    }
}
