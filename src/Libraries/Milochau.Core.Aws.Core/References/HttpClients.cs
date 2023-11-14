using Polly.Extensions.Http;
using Polly;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Http;

namespace Milochau.Core.Aws.Core.References
{
    internal static class HttpClients
    {
        private static readonly IAsyncPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, retryAttempt), 20))); // Max 20 seconds
        private static readonly SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler
        {
            // Fix for https://github.com/aws/aws-lambda-dotnet/issues/1231. HttpClient by default supports only ASCII characters in headers. Changing it to allow UTF8 characters.
            RequestHeaderEncodingSelector = delegate { return Encoding.UTF8; },
            PooledConnectionLifetime = TimeSpan.FromMinutes(30), // Recreate every 30 minutes
        };
        private static readonly PolicyHttpMessageHandler policyHttpMessageHandler = new PolicyHttpMessageHandler(retryPolicy)
        {
            InnerHandler = socketsHttpHandler,
        };

        /// <summary>HTTP client</summary>
        /// <remarks>An infinite timeout is defined - use a CancellationTokenSource around each request.</remarks>
        internal static HttpClient HttpClient { get; } = new HttpClient(policyHttpMessageHandler)
        {
            Timeout = Timeout.InfiniteTimeSpan, // The Lambda container freezes the process at a point where an HTTP request is in progress. We need to make sure we don't timeout waiting for the next invocation.
        };
    }
}
