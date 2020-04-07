using System;
using System.Net.Http;

namespace NotScuffed.Http
{
    static class PrivateExtensions
    {
        private const string TimeoutPropertyKey = "RequestTimeout";

        public static void SetTimeout(this HttpRequestMessage requestMessage, TimeSpan? timeout)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            requestMessage.Properties[TimeoutPropertyKey] = timeout;
        }

        public static TimeSpan? GetTimeout(this HttpRequestMessage requestMessage)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            if (requestMessage.Properties.TryGetValue(
                    TimeoutPropertyKey,
                    out var value)
                && value is TimeSpan timeout)
                return timeout;
            return null;
        }
    }
}