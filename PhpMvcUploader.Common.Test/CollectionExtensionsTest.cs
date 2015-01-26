using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace PhpMvcUploader.Common.Test
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        private IEnumerable<int> _firstTenIntegers;

        [SetUp]
        public void BeforeEach()
        {
            _firstTenIntegers = Enumerable.Range(0, 10);
        }

        [Test]
        public void ForEachWorks()
        {
            var toPopulate = new List<string>();
            var expected = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList();

            _firstTenIntegers.ForEach(i => toPopulate.Add(i.ToString()));

            Assert.That(toPopulate, Is.EquivalentTo(expected));
        }

        [Test]
        public void RandomWorks()
        {
            var source = _firstTenIntegers.Select(i => i * 2 + 1).ToList();

            var result = source.Random();

            Assert.That(source.Contains(result));
        }

        [Test]
        public void CopyWorks()
        {
            var source = new byte[200];
            new Random().NextBytes(source);

            var result = source.Copy();

            Assert.That(source, Is.Not.SameAs(result));
            Assert.That(source, Is.EqualTo(result));
        }
    }
}
