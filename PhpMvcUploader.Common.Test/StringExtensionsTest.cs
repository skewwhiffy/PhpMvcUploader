using System.Collections.Generic;
using NUnit.Framework;

namespace PhpMvcUploader.Common.Test
{
    [TestFixture]
    public class StringExtensionsTest
    {
        const string Template = "{0} is first, {1} is second";
        const string First = "first";
        const string Second = "second";

        private IEnumerable<string> Collection
        {
            get
            {
                yield return First;
                yield return Second;
            }
        }

        [Test]
        public void FormatWorks()
        {
            var result = Template.FormatX(First, Second);

            Assert.That(result, Is.EqualTo(string.Format(Template, First, Second)));
        }

        [Test]
        public void JoinWorks()
        {
            var result = Collection.JoinX();

            Assert.That(result, Is.EqualTo(string.Join(", ", Collection)));
        }

        [Test]
        public void JoinWorksWithSeparator()
        {
            const string separator = " ";

            var result = Collection.JoinX(separator);

            Assert.That(result, Is.EqualTo(string.Join(separator, Collection)));
        }

        [Test]
        public void IsNullOrEmptyWorksForNull()
        {
            Assert.That((null as string).IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmptyWorksForEmpty()
        {
            Assert.That(string.Empty.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmptyWorks()
        {
            Assert.That("hello".IsNullOrEmpty(), Is.False);
        }
    }
}
