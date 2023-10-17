using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Bootstrap
{
    public delegate Task<InvocationResponse> LambdaBootstrapHandler(InvocationRequest invocation);

    /// <summary>
    /// Class to communicate with the Lambda Runtime API, handle initialization,
    /// and run the invoke loop for an AWS Lambda function
    /// </summary>
    public class LambdaBootstrap
    {
        private static readonly HttpClient httpClient = ConstructHttpClient();
        private readonly LambdaBootstrapHandler handler;

        internal IRuntimeApiClient Client { get; set; }

        /// <summary>
        /// Create a LambdaBootstrap that will call the given initializer and handler.
        /// </summary>
        /// <param name="handlerWrapper">The HandlerWrapper to call for each invocation of the Lambda function.</param>
        public LambdaBootstrap(HandlerWrapper handlerWrapper)
        {
            handler = handlerWrapper.Handler;
            Client = new RuntimeApiClient(httpClient);
        }

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        /// <returns>A Task that represents the operation.</returns>
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await InvokeOnceAsync(cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Loop cancelled
                }
            }
        }

        internal async Task InvokeOnceAsync(CancellationToken cancellationToken)
        {
            using var invocation = await Client.GetNextInvocationAsync(cancellationToken);
            InvocationResponse response = null;

            try
            {
                response = await handler(invocation);
            }
            catch (Exception exception)
            {
                WriteUnhandledExceptionToLog(exception);
                await Client.ReportInvocationErrorAsync(invocation.LambdaContext.AwsRequestId, exception);
                return;
            }

            try
            {
                await Client.SendResponseAsync(invocation.LambdaContext.AwsRequestId, response?.OutputStream);
            }
            finally
            {
                if (response != null && response.DisposeOutputStream)
                {
                    response.OutputStream?.Dispose();
                }
            }
        }

        /// <summary>
        /// Utility method for creating an HttpClient used by LambdaBootstrap to interact with the Lambda Runtime API.
        /// </summary>
        /// <returns></returns>
        public static HttpClient ConstructHttpClient()
        {
            // Create the SocketsHttpHandler directly to avoid spending cold start time creating the wrapper HttpClientHandler
            var handler = new SocketsHttpHandler
            {
                // Fix for https://github.com/aws/aws-lambda-dotnet/issues/1231. HttpClient by default supports only ASCII characters in headers. Changing it to allow UTF8 characters.
                RequestHeaderEncodingSelector = delegate { return System.Text.Encoding.UTF8; }
            };

            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromHours(12), // The Lambda container freezes the process at a point where an HTTP request is in progress. We need to make sure we don't timeout waiting for the next invocation.
            };
        }

        private static void WriteUnhandledExceptionToLog(Exception exception)
        {
            // Console.Error.WriteLine are redirected to the IConsoleLoggerWriter which
            // will take care of writing to the function's log stream.
            Console.Error.WriteLine(exception);
        }
    }
}
