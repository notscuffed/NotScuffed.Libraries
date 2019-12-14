using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NotScuffed.Http
{
    public class TimeoutHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            using var cancellationTokenSource = GetCancellationTokenSource(requestMessage, cancellationToken);

            try
            {
                return await base.SendAsync(
                    requestMessage,
                    cancellationTokenSource?.Token ?? cancellationToken);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException();
            }
        }

        private CancellationTokenSource GetCancellationTokenSource(
            HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            var timeout = requestMessage.GetTimeout() ?? Requester.DefaultTimeout;

            if (timeout == Timeout.InfiniteTimeSpan)
                return null;

            var cancellationTokenSource = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken);

            cancellationTokenSource.CancelAfter(timeout);
            return cancellationTokenSource;
        }
    }
}