using FakeItEasy;
using NUnit.Framework;
using PhpMvcUploader.Common;
using PhpMvcUploader.Core.Comparison;

namespace PhpMvcUploader.Core.Test.Comparison
{
    [TestFixture]
    public class FileComparerTest
    {
        private IStreamComparer _streamComparer;

        private FileComparer _comparer;

        [SetUp]
        public void BeforeEach()
        {
            _streamComparer = A.Fake<IStreamComparer>();
            _comparer = new FileComparer(_streamComparer);
        }

        [Test]
        public void TextFilesAreComparedUsingTextCompare()
        {
            FileComparer
                .TextExtensions
                .ForEach(e => {});
        }
    }
}
