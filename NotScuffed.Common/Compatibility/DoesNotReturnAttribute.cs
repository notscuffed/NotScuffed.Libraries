#if !NET5_0_OR_GREATER
using System;

namespace NotScuffed.Common.Compatibility;

public class DoesNotReturnAttribute : Attribute
{
}
#endif