using System.Runtime.CompilerServices;

namespace NotScuffed.Strings
{
    public static class CharacterExtensions
    {
        /// <summary>
        /// Returns true if character is a printable ascii character (doesn't include control characters like \r, \n, \t...)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsASCII(this char c)
        {
            return c >= 0x20 && c <= 0x7E;
        }
    }
}