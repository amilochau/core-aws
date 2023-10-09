using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Helpers;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context
{
    internal class LambdaContext : ILambdaContext
    {
        private readonly LambdaEnvironment _lambdaEnvironment;
        private readonly RuntimeApiHeaders _runtimeApiHeaders;
        private readonly IConsoleLoggerWriter _consoleLogger;

        public LambdaContext(RuntimeApiHeaders runtimeApiHeaders, LambdaEnvironment lambdaEnvironment, IConsoleLoggerWriter consoleLogger)
        {

            _lambdaEnvironment = lambdaEnvironment;
            _runtimeApiHeaders = runtimeApiHeaders;
            _consoleLogger = consoleLogger;

            // set environment variable so that if the function uses the XRay client it will work correctly
            LambdaEnvironment.SetXAmznTraceId(_runtimeApiHeaders.TraceId);
        }

        public string AwsRequestId => _runtimeApiHeaders.AwsRequestId;

        public ILambdaLogger Logger => new LambdaConsoleLogger(_consoleLogger);
    }
}
