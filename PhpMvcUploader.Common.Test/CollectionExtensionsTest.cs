using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace PhpMvcUploader.Common.Test
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        [Test]
        public void ForEach()
        {
            var firstTenIntegers = Enumerable.Range(0, 10);
            var toPopulate = new List<string>();
            var expected = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList();

            firstTenIntegers.ForEach(i => toPopulate.Add(i.ToString()));

            Assert.That(toPopulate, Is.EquivalentTo(expected));
        }
    }
}
