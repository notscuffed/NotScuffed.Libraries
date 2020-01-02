using System.Collections.Generic;
using NotScuffed.Linq;
using NUnit.Framework;

namespace NotScuffed.Tests
{
    public class LinqTests
    {
        [Test]
        [TestCase(new[] {0, 1, 2}, ' ', null, null, ExpectedResult = "0 1 2")]
        [TestCase(new[] {0, 1, 2}, ' ', 1, null, ExpectedResult = "1 2")]
        [TestCase(new[] {0, 1, 2}, ' ', 1, 1, ExpectedResult = "1")]
        public string JoinStringInt(int[] input, char separator, int? startIndex, int? count)
            => JoinString(input, separator, startIndex, count);

        [Test]
        [TestCase(new[] {"ab", "cd", "ef"}, ' ', null, null, ExpectedResult = "ab cd ef")]
        [TestCase(new[] {"ab", "cd", "ef"}, ' ', 1, null, ExpectedResult = "cd ef")]
        [TestCase(new[] {"ab", "cd", "ef"}, ' ', 1, 1, ExpectedResult = "cd")]
        public string JoinStringStr(string[] input, char separator, int? startIndex, int? count)
            => JoinString(input, separator, startIndex, count);

        // Using test cases on method with generic parameters fails in rider for some reason
        private static string JoinString<T>(ICollection<T> input, char separator, int? startIndex, int? count)
        {
            // Test both char & string separator at the same time
            var result1 = JoinStringChar(input, separator, startIndex, count);
            var result2 = JoinStringStr(input, separator.ToString(), startIndex, count);

            Assert.AreEqual(result1, result2);

            return result1;
        }

        private static string JoinStringChar<T>(IEnumerable<T> input, char separator, int? startIndex, int? count)
        {
            if (startIndex.HasValue)
            {
                return count.HasValue
                    ? input.JoinString(separator, startIndex.Value, count.Value)
                    : input.JoinString(separator, startIndex.Value);
            }

            return input.JoinString(separator);
        }

        private static string JoinStringStr<T>(IEnumerable<T> input, string separator, int? startIndex, int? count)
        {
            if (startIndex.HasValue)
            {
                return count.HasValue
                    ? input.JoinString(separator, startIndex.Value, count.Value)
                    : input.JoinString(separator, startIndex.Value);
            }

            return input.JoinString(separator);
        }
    }
}