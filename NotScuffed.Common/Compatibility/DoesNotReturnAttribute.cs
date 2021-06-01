using System;

namespace NotScuffed.Common.Compatibility
{
#if (NETSTANDARD2_0 || NETCOREAPP3_1)
    public class DoesNotReturnAttribute : Attribute
    {
    }
#endif
}