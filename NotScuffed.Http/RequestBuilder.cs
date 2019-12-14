using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NotScuffed.Http
{
    public class RequestBuilder
    {
        private readonly HttpMethod _method;
        private readonly string _uriPathName;
        private readonly Dictionary<string, object> _params;
        private readonly Dictionary<string, object> _headers;
        private readonly Dictionary<string, string> _posts;
        private TimeSpan? Timeout;

        public RequestBuilder(HttpMethod httpMethod, string uriPathName)
        {
            _method = httpMethod;
            _uriPathName = uriPathName;

            _params = new Dictionary<string, object>();
            _headers = new Dictionary<string, object>();
            _posts = new Dictionary<string, string>();

            Timeout = Requester.DefaultTimeout;
        }

        public RequestBuilder AddPost(string param, object value)
        {
            if (param is null)
                throw new ArgumentNullException(nameof(param));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _posts[param] = value.ToString();

            return this;
        }

        public RequestBuilder AddParam(string param, object value)
        {
            if (param is null)
                throw new ArgumentNullException(nameof(param));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _params[param] = value.ToString();

            return this;
        }

        public RequestBuilder AddHeader(string header, object value)
        {
            if (header is null)
                throw new ArgumentNullException(nameof(header));

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _headers[header] = value.ToString();

            return this;
        }

        public RequestBuilder SetTimeout(TimeSpan? timeout)
        {
            Timeout = timeout;
            return this;
        }

        public Task<HttpResponseMessage> Request(Uri baseAddress, IWebProxy webProxy)
        {
            if (baseAddress is null)
                throw new ArgumentNullException(nameof(baseAddress));

            if (webProxy is null)
                throw new ArgumentNullException(nameof(webProxy));

            return webProxy.SendAsync(Build(), Timeout);
        }

        public Task<HttpResponseMessage> Request(HttpClient httpClient)
        {
            if (httpClient is null)
                throw new ArgumentNullException(nameof(httpClient));

            if (!httpClient.HasOperationStarted())
                httpClient.Timeout = System.Threading.Timeout.InfiniteTimeSpan;

            return httpClient.SendAsync(Build(), HttpCompletionOption.ResponseHeadersRead);
        }

        private HttpRequestMessage Build(Uri baseAddress = null)
        {
            var uri = BuildUri(baseAddress);

            var message = new HttpRequestMessage
            {
                RequestUri = baseAddress == null ? new Uri(uri, UriKind.Relative) : new Uri(uri),
                Method = _method
            };

            if (Timeout != null)
                message.SetTimeout(Timeout);

            foreach (var (header, value) in _headers)
            {
                message.Headers.Add(header, value.ToString());
            }

            if (_method == HttpMethod.Post)
                message.Content = new FormUrlEncodedContent(_posts);

            return message;
        }

        private string BuildUri(Uri baseAddress = null)
        {
            var uri = string.Empty;

            if (baseAddress != null)
                uri = $"{baseAddress.Scheme}://{baseAddress.Host}";

            if (_params.Count <= 0)
                return uri + _uriPathName;

            var parameters = _params.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value.ToString())}");
            var uriParameters = string.Join("&", parameters);

            return uri + $"{_uriPathName}?{uriParameters}";
        }
    }
}