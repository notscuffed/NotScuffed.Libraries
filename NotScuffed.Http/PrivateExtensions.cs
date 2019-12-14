using System;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;

namespace NotScuffed.Http
{
    static class PrivateExtensions
    {
        private const string TimeoutPropertyKey = "RequestTimeout";

        public static bool HasOperationStarted(this HttpClient client) => HasOperationStartedDelegate(client);
        private static readonly HttpClientHasOperationStarted HasOperationStartedDelegate;

        static PrivateExtensions()
        {
            var type = typeof(HttpClient);

            var field = type.GetField("_operationStarted", BindingFlags.Instance | BindingFlags.NonPublic);

            var method = new DynamicMethod("HasOperationStarted", typeof(bool), new[] {typeof(HttpClient)});
            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, field);
            ilGenerator.Emit(OpCodes.Ret);

            HasOperationStartedDelegate =
                (HttpClientHasOperationStarted) method.CreateDelegate(typeof(HttpClientHasOperationStarted));
        }

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

        private delegate bool HttpClientHasOperationStarted(HttpClient client);
    }
}