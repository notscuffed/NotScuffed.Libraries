using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace NotScuffed.Http
{
    public class RequestBuilder
    {
        private readonly HttpMethod _method;
        private readonly Uri _uri;
        private Dictionary<string, string> _query;
        private Dictionary<string, string> _headers;
        private Dictionary<string, string> _posts;
        private HttpContent _httpContent;
        private Dictionary<HttpStatusCode, int> _retryOnStatusCode;
        private HttpRequestMessage _message;
        private TimeSpan? _timeout;

        public RequestBuilder(HttpMethod httpMethod, string uri)
        {
            _method = httpMethod;
            _uri = new Uri(uri, UriKind.RelativeOrAbsolute);

            _timeout = Requester.DefaultTimeout;
        }

        public RequestBuilder AddPost(string param, object value)
        {
            if (param is null)
                throw new ArgumentNullException(nameof(param));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (_posts == null)
                _posts = new Dictionary<string, string>();

            _posts[param] = value.ToString();

            return this;
        }

        public RequestBuilder AddQuery(string query, object value)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (_query == null)
                _query = new Dictionary<string, string>();

            _query[query] = value.ToString();

            return this;
        }

        public RequestBuilder AddHeader(string header, object value)
        {
            if (header is null)
                throw new ArgumentNullException(nameof(header));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (_headers == null)
                _headers = new Dictionary<string, string>();

            _headers[header] = value.ToString();

            return this;
        }

        public RequestBuilder SetTimeout(TimeSpan? timeout)
        {
            _timeout = timeout;
            return this;
        }

        public RequestBuilder SetJsonBody(string json)
        {
            _httpContent = new StringContent(json);

            _httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return this;
        }

        /// <summary>
        /// Set retry attempts count on selected status code
        /// </summary>
        /// <param name="statusCode">Status code to retry on</param>
        /// <param name="retryCount">Retry count. Set to -1 for infinite retries</param>
        /// <returns></returns>
        public RequestBuilder SetRetryOnStatusCode(HttpStatusCode statusCode, int retryCount)
        {
            if (_retryOnStatusCode == null)
                _retryOnStatusCode = new Dictionary<HttpStatusCode, int>();

            _retryOnStatusCode[statusCode] = retryCount;

            return this;
        }

        public Task<HttpResponseMessage> Request(Uri baseAddress, IWebProxy webProxy)
        {
            if (baseAddress is null)
                throw new ArgumentNullException(nameof(baseAddress));

            if (webProxy is null)
                throw new ArgumentNullException(nameof(webProxy));

            Build();

            if (_retryOnStatusCode == null)
                return webProxy.SendAsync(_message, _timeout);

            return RetryRequest(t => webProxy.SendAsync(_message, _timeout, t));
        }

        public Task<HttpResponseMessage> Request(HttpClient httpClient, CancellationToken cancellationToken = default)
        {
            if (httpClient is null)
                throw new ArgumentNullException(nameof(httpClient));

            Build();

            if (_retryOnStatusCode == null)
                return httpClient.SendAsync(_message, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return RetryRequest(
                t => httpClient.SendAsync(_message, HttpCompletionOption.ResponseHeadersRead, t),
                cancellationToken
            );
        }

        private async Task<HttpResponseMessage> RetryRequest(
            Func<CancellationToken, Task<HttpResponseMessage>> requestFunction,
            CancellationToken cancellationToken = default)
        {
            var statusCodes = _retryOnStatusCode.Keys.ToArray();

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                var response = await requestFunction(cancellationToken);
                var statusCode = response.StatusCode;

                if (!statusCodes.Contains(statusCode))
                    return response;

                var retryCountLeft = _retryOnStatusCode[statusCode] = _retryOnStatusCode[statusCode]--;

                if (retryCountLeft < 0)
                    return response;
            }
        }

        private void Build()
        {
            var message = new HttpRequestMessage
            {
                RequestUri = BuildUri(_uri, _query),
                Method = _method,
            };

            if (_timeout != null)
                message.SetTimeout(_timeout);

            if (_httpContent != null)
                message.Content = _httpContent;
            else if (_posts != null)
                message.Content = new FormUrlEncodedContent(_posts);

            if (_headers != null)
            {
                foreach (var pair in _headers)
                {
                    message.Headers.Add(pair.Key, pair.Value);
                }
            }

            _message = message;
        }

        public static Uri BuildUri(Uri path, Dictionary<string, string> query)
        {
            if (!path.IsAbsoluteUri)
                return new Uri(path.OriginalString + "?" + BuildQuery(query), UriKind.RelativeOrAbsolute);

            var builder = new UriBuilder(path);

            if (query != null) builder.Query = BuildQuery(query);

            return builder.Uri;
        }

        public static string BuildQuery(Dictionary<string, string> query)
        {
            if (query == null)
                return string.Empty;

            var queryBuilder = new QueryBuilder();

            foreach (var pair in query)
            {
                queryBuilder.Add(pair.Key, pair.Value);
            }

            return queryBuilder.ToQueryString().Value.TrimStart('?');
        }
    }
}