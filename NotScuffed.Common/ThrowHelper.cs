using System;
using System.IO;
using System.Xml;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;

#else
using NotScuffed.Common.Compatibility;
#endif

namespace NotScuffed.Common;

public static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowException(string message)
    {
        throw new Exception(message);
    }

    [DoesNotReturn]
    public static void ThrowOutOfRangeException(string paramName)
    {
        throw new ArgumentOutOfRangeException(paramName);
    }

    [DoesNotReturn]
    public static void ThrowException(this XmlTextReader reader, string message)
    {
        throw new Exception($"{message} at line {reader.LineNumber}:{reader.LinePosition}");
    }

    [DoesNotReturn]
    public static void ThrowNotImplementedException()
    {
        throw new NotImplementedException();
    }

    [DoesNotReturn]
    public static T ThrowNotImplementedException<T>()
    {
        throw new NotImplementedException();
    }

    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string message = null)
    {
        throw new InvalidOperationException(message);
    }

    [DoesNotReturn]
    public static void ThrowArgumentNullException(string paramName)
    {
        throw new ArgumentNullException(paramName);
    }

    [DoesNotReturn]
    public static void ThrowArgumentException(string paramName)
    {
        throw new ArgumentException(paramName);
    }

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string paramName)
    {
        throw new ArgumentOutOfRangeException(paramName);
    }

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string paramName, object actualValue, string message = null)
    {
        throw new ArgumentOutOfRangeException(paramName, actualValue, message);
    }

    [DoesNotReturn]
    public static void ThrowObjectDisposedException(string objectName = null)
    {
        throw new ObjectDisposedException(objectName);
    }

    [DoesNotReturn]
    public static void ThrowNotSupportedException(string message = null)
    {
        throw new NotSupportedException(message);
    }

    [DoesNotReturn]
    public static void ThrowOverflowException(string message = null)
    {
        throw new OverflowException(message);
    }

    [DoesNotReturn]
    public static void ThrowIOException(string message)
    {
        throw new IOException(message);
    }
}