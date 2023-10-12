using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Client;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Helpers;
using Milochau.Core.Aws.Core.References;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context
{
    internal class LambdaContext : ILambdaContext
    {
        private readonly RuntimeApiHeaders _runtimeApiHeaders;
        private readonly IConsoleLoggerWriter _consoleLogger;

        public LambdaContext(RuntimeApiHeaders runtimeApiHeaders, IConsoleLoggerWriter consoleLogger)
        {

            _runtimeApiHeaders = runtimeApiHeaders;
            _consoleLogger = consoleLogger;

            // set environment variable so that if the function uses the XRay client it will work correctly
            EnvironmentVariables.SetEnvironmentVariable(EnvironmentVariables.Key_TraceId, _runtimeApiHeaders.TraceId);
        }

        public string AwsRequestId => _runtimeApiHeaders.AwsRequestId;

        public ILambdaLogger Logger => new LambdaConsoleLogger(_consoleLogger);
    }
}
