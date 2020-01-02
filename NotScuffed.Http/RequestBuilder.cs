using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NotScuffed.Http
{
    public class RequestBuilder
    {
        private readonly HttpMethod _method;
        private readonly string _uriPathName;
        private Dictionary<string, object> _params;
        private Dictionary<string, object> _headers;
        private Dictionary<string, string> _posts;
        private Dictionary<HttpStatusCode, int> _retryOnStatusCode;
        private HttpRequestMessage _message;
        private TimeSpan? _timeout;

        public RequestBuilder(HttpMethod httpMethod, string uriPathName)
        {
            _method = httpMethod;
            _uriPathName = uriPathName;

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

        public RequestBuilder AddParam(string param, object value)
        {
            if (param is null)
                throw new ArgumentNullException(nameof(param));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (_params == null)
                _params = new Dictionary<string, object>();

            _params[param] = value.ToString();

            return this;
        }

        public RequestBuilder AddHeader(string header, object value)
        {
            if (header is null)
                throw new ArgumentNullException(nameof(header));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (_headers == null)
                _headers = new Dictionary<string, object>();

            _headers[header] = value.ToString();

            return this;
        }

        public RequestBuilder SetTimeout(TimeSpan? timeout)
        {
            _timeout = timeout;
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

            return RetryRequest(() => webProxy.SendAsync(_message, _timeout));
        }

        public Task<HttpResponseMessage> Request(HttpClient httpClient)
        {
            if (httpClient is null)
                throw new ArgumentNullException(nameof(httpClient));

            if (!httpClient.HasOperationStarted())
                httpClient.Timeout = System.Threading.Timeout.InfiniteTimeSpan;

            Build();

            if (_retryOnStatusCode == null)
                return httpClient.SendAsync(_message, HttpCompletionOption.ResponseHeadersRead);

            return RetryRequest(() => httpClient.SendAsync(_message, HttpCompletionOption.ResponseHeadersRead));
        }

        private async Task<HttpResponseMessage> RetryRequest(Func<Task<HttpResponseMessage>> requestFunction)
        {
            var statusCodes = _retryOnStatusCode.Keys.ToArray();

            while (true)
            {
                var response = await requestFunction();
                var statusCode = response.StatusCode;

                if (!statusCodes.Contains(statusCode))
                    return response;

                var retryCountLeft = _retryOnStatusCode[statusCode] = _retryOnStatusCode[statusCode]--;

                if (retryCountLeft < 0)
                    return response;
            }
        }

        private void Build(Uri baseAddress = null)
        {
            var uri = BuildUri(baseAddress);

            var message = new HttpRequestMessage
            {
                RequestUri = baseAddress == null ? new Uri(uri, UriKind.Relative) : new Uri(uri),
                Method = _method
            };

            if (_timeout != null)
                message.SetTimeout(_timeout);

            if (_headers != null)
            {
                foreach (var (header, value) in _headers)
                {
                    message.Headers.Add(header, value.ToString());
                }
            }

            if (_posts != null)
                message.Content = new FormUrlEncodedContent(_posts);

            _message = message;
        }

        private string BuildUri(Uri baseAddress = null)
        {
            var uri = string.Empty;

            if (baseAddress != null)
                uri = $"{baseAddress.Scheme}://{baseAddress.Host}";

            if (_params == null || _params.Count == 0)
                return uri + _uriPathName;

            var parameters = _params
                .Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value.ToString())}");

            return uri + $"{_uriPathName}?{string.Join("&", parameters)}";
        }
    }
}