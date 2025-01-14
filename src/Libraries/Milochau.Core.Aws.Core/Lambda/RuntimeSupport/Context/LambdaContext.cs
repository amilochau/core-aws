using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using Milochau.Core.Aws.Core.References;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context
{
    internal class LambdaContext : ILambdaContext
    {
        private readonly RuntimeApiHeaders runtimeApiHeaders;

        public LambdaContext(RuntimeApiHeaders runtimeApiHeaders)
        {
            this.runtimeApiHeaders = runtimeApiHeaders;
            Logger = new LambdaConsoleLogger(this.runtimeApiHeaders.AwsRequestId);

            // set environment variable so that if the function uses the XRay client it will work correctly
            Logger.LogLine(Microsoft.Extensions.Logging.LogLevel.Warning, $"Set Environment variable (traceId) - Old value is {EnvironmentVariables.TraceId} - New value is {this.runtimeApiHeaders.TraceId}");
            EnvironmentVariables.TraceId = this.runtimeApiHeaders.TraceId;

            Logger.LogLine(Microsoft.Extensions.Logging.LogLevel.Warning, $"Set Environment variable (requestId) - Old value is {EnvironmentVariables.RequestId} - New value is {this.runtimeApiHeaders.AwsRequestId}");
            EnvironmentVariables.RequestId = this.runtimeApiHeaders.AwsRequestId;
        }

        public string AwsRequestId => runtimeApiHeaders.AwsRequestId;

        public ILambdaLogger Logger { get; }
    }
}
