using System;
using System.Threading;

namespace NotScuffed.Threading
{
    public static class Debouncer
    {
        public static Action Debounce(Action action, TimeSpan timeSpan)
        {
            Timer timer = null;

            return () =>
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                timer = new Timer(_ => action(), null, timeSpan, TimeSpan.FromMilliseconds(-1));
            };
        }
        
        public static Action<T1> Debounce<T1>(Action<T1> action, TimeSpan timeSpan)
        {
            Timer timer = null;

            return (T1 a) =>
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                timer = new Timer(_ => action(a), null, timeSpan, TimeSpan.FromMilliseconds(-1));
            };
        }
        
        public static Action<T1, T2> Debounce<T1, T2>(Action<T1, T2> action, TimeSpan timeSpan)
        {
            Timer timer = null;

            return (T1 a, T2 b) =>
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                timer = new Timer(_ => action(a, b), null, timeSpan, TimeSpan.FromMilliseconds(-1));
            };
        }
        
        public static Action<T1, T2, T3> Debounce<T1, T2, T3>(Action<T1, T2, T3> action, TimeSpan timeSpan)
        {
            Timer timer = null;

            return (T1 a, T2 b, T3 c) =>
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                timer = new Timer(_ => action(a, b, c), null, timeSpan, TimeSpan.FromMilliseconds(-1));
            };
        }
    }
}