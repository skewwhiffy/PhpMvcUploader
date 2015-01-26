using System;
using System.Threading;

namespace PhpMvcUploader.Common
{
    public static class RobustificationExtension
    {
        public static void TimesTry(this int times, Action action)
        {
            Exception ex = null;
            for (int i = 0; i < times; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch(Exception e)
                {
                    ex = e;
                }
            }
            ex = ex ?? new InvalidOperationException("The universe has broken");
            throw new TimeoutException("Tried {0} times, failed with message: {1}".FormatX(times, ex.Message), ex);
        }

        public static Tryer TimesEvery(this int times, TimeSpan timespan)
        {
            return new Tryer(times, timespan);
        }

        public class Tryer
        {
            private readonly int _times;
            private readonly TimeSpan _timespan;

            public Tryer(int times, TimeSpan timespan)
            {
                _times = times;
                _timespan = timespan;
            }

            public void Try(Action action)
            {
                bool firstTime = true;
                _times.TimesTry(() =>
                {
                    if (firstTime)
                    {
                        firstTime = false;
                    }
                    else
                    {
                        Thread.Sleep(_timespan);
                    }
                    action();
                });
            }
        }

        public static T NullGuard<T>(this T item)
            where T : class
        {
            if (item == null)
            {
                throw new NullReferenceException(typeof (T).FullName);
            }
            return item;
        }
    }
}
