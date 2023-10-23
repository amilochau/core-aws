using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using System;
using System.IO;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Bootstrap
{
    public delegate Task<InvocationResponse> LambdaBootstrapHandler(InvocationRequest invocation);

    /// <summary>
    /// Class to communicate with the Lambda Runtime API, handle initialization,
    /// and run the invoke loop for an AWS Lambda function
    /// </summary>
    public static class LambdaBootstrap
    {
        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        public static Task RunAsync(Func<Stream, ILambdaContext, Task> handler)
        {
            var handlerWrapper = HandlerWrapper.GetHandlerWrapper(handler);
            return RunAsync(handlerWrapper.Handler);
        }

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        public static Task RunAsync(Func<Stream, ILambdaContext, Task<Stream>> handler)
        {
            var handlerWrapper = HandlerWrapper.GetHandlerWrapper(handler);
            return RunAsync(handlerWrapper.Handler);
        }

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        public static Task RunAsync<TRequest>(Func<TRequest, ILambdaContext, Task> handler, JsonTypeInfo<TRequest> requestInfo)
        {
            var handlerWrapper = HandlerWrapper.GetHandlerWrapper(handler, requestInfo);
            return RunAsync(handlerWrapper.Handler);
        }

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        public static Task RunAsync<TRequest, TResponse>(Func<TRequest, ILambdaContext, Task<TResponse>> handler, JsonTypeInfo<TRequest> requestInfo, JsonTypeInfo<TResponse> responseInfo)
        {
            var handlerWrapper = HandlerWrapper.GetHandlerWrapper(handler, requestInfo, responseInfo);
            return RunAsync(handlerWrapper.Handler);
        }

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        private async static Task RunAsync(LambdaBootstrapHandler handler)
        {
            var runtimeApiClient = new RuntimeApiClient();
            while (true)
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromHours(12));
                await InvokeOnceAsync(runtimeApiClient, handler, cts.Token);
            }
        }

        private async static Task InvokeOnceAsync(IRuntimeApiClient runtimeApiClient, LambdaBootstrapHandler handler, CancellationToken cancellationToken)
        {
            using var invocation = await runtimeApiClient.GetNextInvocationAsync(cancellationToken);
            InvocationResponse response = null;

            try
            {
                response = await handler(invocation);
            }
            catch (Exception exception)
            {
                invocation.LambdaContext.Logger.LogLineError(Microsoft.Extensions.Logging.LogLevel.Error, exception.ToString());
                await runtimeApiClient.ReportInvocationErrorAsync(invocation.LambdaContext.AwsRequestId, exception, cancellationToken);
                return;
            }

            try
            {
                response.OutputStream.Position = 0;
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
    }
}
