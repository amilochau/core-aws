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
            Logger = new LambdaConsoleLogger();

            // set environment variable so that if the function uses the XRay client it will work correctly
            var oldTraceId = EnvironmentVariables.TraceId;
            var oldRequestIId = EnvironmentVariables.RequestId;
            EnvironmentVariables.TraceId = this.runtimeApiHeaders.TraceId;
            EnvironmentVariables.RequestId = this.runtimeApiHeaders.AwsRequestId;
        }

        public string AwsRequestId => runtimeApiHeaders.AwsRequestId;

        public ILambdaLogger Logger { get; }
    }
}
