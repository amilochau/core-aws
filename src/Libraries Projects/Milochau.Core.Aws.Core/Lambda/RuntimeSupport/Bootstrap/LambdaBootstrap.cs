using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using System;
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
        private readonly LambdaBootstrapHandler handler;
        private readonly IRuntimeApiClient runtimeApiClient;

        /// <summary>
        /// Create a LambdaBootstrap that will call the given initializer and handler.
        /// </summary>
        /// <param name="handlerWrapper">The HandlerWrapper to call for each invocation of the Lambda function.</param>
        public LambdaBootstrap(HandlerWrapper handlerWrapper)
        {
            handler = handlerWrapper.Handler;
            runtimeApiClient = new RuntimeApiClient();
        }

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromHours(12));
                    using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
                    await InvokeOnceAsync(combinedCts.Token);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Loop cancelled
                }
            }
        }

        private async Task InvokeOnceAsync(CancellationToken cancellationToken)
        {
            using var invocation = await runtimeApiClient.GetNextInvocationAsync(cancellationToken);
            InvocationResponse response = null;

            try
            {
                response = await handler(invocation);
            }
            catch (Exception exception)
            {
                WriteUnhandledExceptionToLog(exception);
                await runtimeApiClient.ReportInvocationErrorAsync(invocation.LambdaContext.AwsRequestId, exception, cancellationToken);
                return;
            }

            try
            {
                await runtimeApiClient.SendResponseAsync(invocation.LambdaContext.AwsRequestId, response.OutputStream, cancellationToken);
            }
            finally
            {
                if (response != null && response.DisposeOutputStream)
                {
                    response.OutputStream?.Dispose();
                }
            }
        }

        private static void WriteUnhandledExceptionToLog(Exception exception)
        {
            // Console.Error.WriteLine are redirected to the IConsoleLoggerWriter which
            // will take care of writing to the function's log stream.
            Console.Error.WriteLine(exception);
        }
    }
}
