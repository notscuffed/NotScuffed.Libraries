using System.Runtime.CompilerServices;

namespace NotScuffed.Common
{
    public static class ArgGuard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull<T>(T value, string name)
        {
            if (value != null)
                return;

            ThrowHelper.ThrowArgumentNullException(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfFalse(bool value, string name)
        {
            if (value)
                return;

            ThrowHelper.ThrowArgumentException(name);
        }
    }
}
