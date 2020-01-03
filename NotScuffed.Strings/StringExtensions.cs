using System;
using System.Collections.Generic;

namespace NotScuffed.Strings
{
    public static class StringExtensions
    {
        /// <summary>
        /// Splits camel case words (example: SomeWord => ["Some", "Word"])
        /// </summary>
        public static string[] SplitCamelCase(this string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length <= 1)
                return new[] {input};

            var words = new List<string>();
            var lastIndex = 0;

            for (var i = 1; i < input.Length; i++)
            {
                if (!char.IsUpper(input[i]))
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