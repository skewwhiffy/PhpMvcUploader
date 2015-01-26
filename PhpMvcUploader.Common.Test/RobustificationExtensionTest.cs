using System;
using NUnit.Framework;

namespace PhpMvcUploader.Common.Test
{
    [TestFixture]
    public class RobustificationExtensionTest
    {
        private int _count;
        private bool _touched;
        private Action _action;
        private TimeSpan _timespan;

        [SetUp]
        public void BeforeEach()
        {
            _count = 0;
            _touched = false;
            _action = () =>
            {
                if (_count <= 3)
                {
                    _count++;
                    throw new Exception();
                }
                _touched = true;
            };
            _timespan = TimeSpan.FromMilliseconds(10);
        }

        [Test]
        public void RetryWorks()
        {
            5.TimesTry(_action);

            Assert.That(_touched);
        }

        [Test]
        public void RetryWithIntervalWorks()
        {
            const int retries = 5;
            var start = DateTime.Now;

            retries.TimesEvery(_timespan).Try(_action);

            var end = DateTime.Now;
            var elapsed = end - start;
            var expectedElapsed = TimeSpan.FromTicks((retries - 1)*_timespan.Ticks);
            Assert.That(elapsed, Is.GreaterThan(expectedElapsed));
            Assert.That(_touched);
        }

        [Test]
        public void NullGuardWorksWhenNotNull()
        {
            "hello".NullGuard();
        }

        [Test]
        public void NullGuardWorksWhenNull()
        {
            Assert.Throws<NullReferenceException>(() => (null as string).NullGuard());
        }
    }
}
