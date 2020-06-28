using System.Collections.Generic;
using System.Linq;

namespace NotScuffed.Linq
{
    public static class JoinStringExtension
    {
        public static string JoinString<T>(this IEnumerable<T> enumerable, char separator)
        {
#if NETSTANDARD2_0
            return string.Join(separator.ToString(), enumerable);
#else
            return string.Join(separator, enumerable);
#endif
        }

        public static string JoinString<T>(this IEnumerable<T> enumerable, string separator)
        {
            return string.Join(separator, enumerable);
        }

        public static string JoinString<T>(this IEnumerable<T> enumerable, char separator, int startIndex, int count)
        {
#if NETSTANDARD2_0
            return string.Join(
                separator.ToString(),
                enumerable
                    .Skip(startIndex)
                    .Take(count)
                    .Select(x => x.ToString())
            );
#else
            return string.Join(
                separator,
                enumerable
                    .Skip(startIndex)
                    .Take(count)
                    .Select(x => x.ToString())
            );
#endif
        }

        public static string JoinString<T>(this IEnumerable<T> enumerable, string separator, int startIndex, int count)
        {
            return string.Join(
                separator,
                enumerable
                    .Skip(startIndex)
                    .Take(count)
                    .Select(x => x.ToString())
            );
        }

        public static string JoinString<T>(this IEnumerable<T> enumerable, char separator, int startIndex)
        {
#if NETSTANDARD2_0
            return string.Join(
                separator.ToString(),
                enumerable
                    .Skip(startIndex)
                    .Select(x => x.ToString())
            );
#else
            return string.Join(
                separator,
                enumerable
                    .Skip(startIndex)
                    .Select(x => x.ToString())
            );
#endif
        }

        public static string JoinString<T>(this IEnumerable<T> enumerable, string separator, int startIndex)
        {
            return string.Join(
                separator,
                enumerable
                    .Skip(startIndex)
                    .Select(x => x.ToString())
            );
        }
    }
}