using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using Milochau.Core.Aws.Core.References;
using System;
using System.IO;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Bootstrap
{
    internal delegate Task<InvocationResponse> LambdaBootstrapHandler(InvocationRequest invocation, CancellationToken cancellationToken);

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
        public static Task RunAsync(Func<Stream, ILambdaContext, CancellationToken, Task> handler) => RunAsync(HandlerWrapper.BuildHandler(handler));

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        public static Task RunAsync<TRequest>(Func<TRequest, ILambdaContext, CancellationToken, Task> handler, JsonTypeInfo<TRequest> requestInfo) => RunAsync(HandlerWrapper.BuildHandler(handler, requestInfo));

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        public static Task RunAsync(Func<Stream, ILambdaContext, CancellationToken, Task<Stream>> handler) => RunAsync(HandlerWrapper.BuildHandler(handler));

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        public static Task RunAsync<TRequest, TResponse>(Func<TRequest, ILambdaContext, CancellationToken, Task<TResponse>> handler, JsonTypeInfo<TRequest> requestInfo, JsonTypeInfo<TResponse> responseInfo) => RunAsync(HandlerWrapper.BuildHandler(handler, requestInfo, responseInfo));

        /// <summary>
        /// Run the initialization Func if provided.
        /// Then run the invoke loop, calling the handler for each invocation.
        /// </summary>
        private async static Task RunAsync(LambdaBootstrapHandler handler)
        {
            AdjustMemorySettings();

            var runtimeApiClient = new RuntimeApiClient();
            while (true)
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromHours(12));
                await InvokeOnceAsync(runtimeApiClient, handler, cts.Token);
            }
        }

        private async static Task InvokeOnceAsync(RuntimeApiClient runtimeApiClient, LambdaBootstrapHandler handler, CancellationToken cancellationToken)
        {
            using var invocation = await runtimeApiClient.GetNextInvocationAsync(cancellationToken);
            InvocationResponse? response = null;

            try
            {
                response = await handler(invocation, cancellationToken);
            }
            catch (Exception ex)
            {
                invocation.LambdaContext.Logger.LogLine(Microsoft.Extensions.Logging.LogLevel.Error, ex.ToString());
                await runtimeApiClient.ReportInvocationErrorAsync(invocation.LambdaContext.AwsRequestId, ex, cancellationToken);
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

        /// <summary>
        /// The .NET runtime does not recognize the memory limits placed by Lambda via Lambda's cgroups. This method is run during startup to inform the
        /// .NET runtime the max memory configured for Lambda function. The max memory can be determined using the AWS_LAMBDA_FUNCTION_MEMORY_SIZE environment variable
        /// which has the memory in MB.
        /// 
        /// For additional context on setting the heap size refer to this GitHub issue:
        /// https://github.com/dotnet/runtime/issues/70601
        /// </summary>
        private static void AdjustMemorySettings()
        {
            try
            {
                if (int.TryParse(EnvironmentVariables.FunctionMemorySize, out var lambdaMemoryInMb))
                {
                    ulong memoryInBytes = (ulong)lambdaMemoryInMb * 1024 * 1024;
                    AppContext.SetData("GCHeapHardLimit", memoryInBytes);
                    GC.RefreshMemoryLimit();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
