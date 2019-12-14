using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NotScuffed.Http
{
    public interface IWebProxy
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, TimeSpan? timeOut = null);
    }
}