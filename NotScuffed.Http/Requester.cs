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
    }
}