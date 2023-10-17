using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Helpers;
using Milochau.Core.Aws.Core.References;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context
{
    internal class LambdaContext : ILambdaContext
    {
        private readonly RuntimeApiHeaders runtimeApiHeaders;
        private readonly IConsoleLoggerWriter consoleLogger;

        public LambdaContext(RuntimeApiHeaders runtimeApiHeaders, IConsoleLoggerWriter consoleLogger)
        {

            this.runtimeApiHeaders = runtimeApiHeaders;
            this.consoleLogger = consoleLogger;

            // set environment variable so that if the function uses the XRay client it will work correctly
            EnvironmentVariables.SetEnvironmentVariable(EnvironmentVariables.Key_TraceId, this.runtimeApiHeaders.TraceId);
        }

        public string AwsRequestId => runtimeApiHeaders.AwsRequestId;

        public ILambdaLogger Logger => new LambdaConsoleLogger(consoleLogger);
    }
}
