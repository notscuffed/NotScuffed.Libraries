using System;
using System.Net.Http;

namespace NotScuffed.Http
{
    public static class Requester
    {
        public static TimeSpan DefaultTimeout = TimeSpan.FromMinutes(1);

        public static HttpClient CreateClient(bool validateSsl = true, Action<HttpClientHandler> processHandler = null)
        {
            return InternalCreateClient(null, validateSsl, processHandler);
        }

        public static HttpClient CreateClient(Uri baseAddress, bool validateSsl = true,
            Action<HttpClientHandler> processHandler = null)
        {
            return InternalCreateClient(baseAddress, validateSsl, processHandler);
        }

        public static HttpClient CreateClient(string baseAddress, bool validateSsl = true,
            Action<HttpClientHandler> processHandler = null)
        {
            return InternalCreateClient(baseAddress == null ? null : new Uri(baseAddress), validateSsl, processHandler);
        }

        private static HttpClient InternalCreateClient(Uri baseAddress, bool validateSsl,
            Action<HttpClientHandler> processHandler)
        {
            var handler = validateSsl ? HttpHandlers.Default : HttpHandlers.NoSSLValidation;

            processHandler?.Invoke((HttpClientHandler) handler.InnerHandler);

            return new HttpClient(handler)
            {
                BaseAddress = baseAddress,
            };
        }

        public static RequestBuilder Get(string uri)
        {
            return new RequestBuilder(HttpMethod.Get, uri);
        }

        public static RequestBuilder Post(string uri)
        {
            return new RequestBuilder(HttpMethod.Post, uri);
        }

        public static RequestBuilder Delete(string uri)
        {
            return new RequestBuilder(HttpMethod.Delete, uri);
        }

        public static RequestBuilder Head(string uri)
        {
            return new RequestBuilder(HttpMethod.Head, uri);
        }

        public static RequestBuilder Options(string uri)
        {
            return new RequestBuilder(HttpMethod.Options, uri);
        }

        public static RequestBuilder Put(string uri)
        {
            return new RequestBuilder(HttpMethod.Put, uri);
        }

        public static RequestBuilder Patch(string uri)
        {
            #if NETSTANDARD2_0
            return new RequestBuilder(new HttpMethod("PATCH"), uri);
            #else
            return new RequestBuilder(HttpMethod.Patch, uri);
            #endif
        }

        public static RequestBuilder Trace(string uri)
        {
            return new RequestBuilder(HttpMethod.Trace, uri);
        }

        public static RequestBuilder FromMethod(HttpMethod method, string uri)
        {
            return new RequestBuilder(method, uri);
        }
    }
}