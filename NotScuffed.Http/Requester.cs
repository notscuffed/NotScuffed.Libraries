using System;
using System.Net.Http;

namespace NotScuffed.Http
{
    public static class Requester
    {
        public static TimeSpan DefaultTimeout = TimeSpan.FromMinutes(1);

        public static HttpClient CreateClient(Uri baseAddress, bool validateSsl = true)
        {
            return new HttpClient(validateSsl ? HttpHandlers.Default : HttpHandlers.NoSSLValidation)
            {
                BaseAddress = baseAddress
            };
        }

        public static HttpClient CreateClient(string baseAddress, bool validateSsl = true)
        {
            return new HttpClient(validateSsl ? HttpHandlers.Default : HttpHandlers.NoSSLValidation)
            {
                BaseAddress = new Uri(baseAddress)
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
            return new RequestBuilder(HttpMethod.Patch, uri);
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