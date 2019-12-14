using System.Net.Http;

namespace NotScuffed.Http
{
    public static class HttpHandlers
    {
        public static readonly DelegatingHandler Default = new TimeoutHandler
        {
            InnerHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
            }
        };

        public static readonly DelegatingHandler NoSSLValidation = new TimeoutHandler
        {
            InnerHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (message, certificate, x509Chain, sslPolicyErrors) => true,
                AllowAutoRedirect = false,
            }
        };
    }
}