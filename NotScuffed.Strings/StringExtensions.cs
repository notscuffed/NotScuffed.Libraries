using System;
using System.Collections.Generic;

namespace NotScuffed.Strings
{
    public static class StringExtensions
    {
        /// <summary>
        /// Splits camel case words (example: Some_Word => ["Some_", "Word"])
        /// </summary>
        public static string[] SplitCamelCase(this string input)
        {
            return SplitCamelCaseImpl(ref input, (str, i) => !char.IsUpper(str[i]));
        }

        /// <summary>
        /// Splits camel case words and on non letters (example: Some_Word => ["Some", "_", "Word"])
        /// </summary>
        public static string[] SplitCamelCaseNonLetter(this string input)
        {
            return SplitCamelCaseImpl(ref input, (str, i) => char.IsLetter(str[i - 1]) && char.IsLower(str[i]));
        }

        private static string[] SplitCamelCaseImpl(ref string input, Func<string, int, bool> predicate)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length <= 1)
                return new[] {input};

            var words = new List<string>();
            var lastIndex = 0;

            for (var i = 1; i < input.Length; i++)
            {
                if (predicate(input, i))
                    continue;

                words.Add(input[lastIndex..i]);
                lastIndex = i;
            }

            if (lastIndex != input.Length)
                words.Add(input[lastIndex..]);

            return words.ToArray();
        }
    }
}