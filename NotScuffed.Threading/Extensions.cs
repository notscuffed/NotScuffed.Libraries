using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NotScuffed.Threading
{
    public static class Extensions
    {
        /// <summary>
        /// Performs the specified action on each element of IEnumerable&lt;T&gt; asynchrously
        /// </summary>
        public static Task ForEachAsync<T>(
            this IEnumerable<T> source,
            int partitionCount,
            Func<T, Task> action)
        {
            return Task.WhenAll(
                Partitioner
                    .Create(source)
                    .GetPartitions(partitionCount)
                    .Select(x => Task.Run(async delegate
                    {
                        using (x)
                        {
                            while (x.MoveNext())
                            {
                                await action(x.Current);
                            }
                        }
                    })));
        }
    }
}