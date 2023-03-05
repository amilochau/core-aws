using System;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context
{
    public class LambdaContext : ILambdaContext
    {
        internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private RuntimeApiHeaders _runtimeApiHeaders;
        private long _deadlineMs;
        private int _memoryLimitInMB;
        private Lazy<CognitoIdentity> _cognitoIdentityLazy;
        private Lazy<CognitoClientContext> _cognitoClientContextLazy;
        private IConsoleLoggerWriter _consoleLogger;

        public LambdaContext(RuntimeApiHeaders runtimeApiHeaders, IConsoleLoggerWriter consoleLogger)
        {
            _runtimeApiHeaders = runtimeApiHeaders;
            _consoleLogger = consoleLogger;

            int.TryParse(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_MEMORY_SIZE"), out _memoryLimitInMB);
            long.TryParse(_runtimeApiHeaders.DeadlineMs, out _deadlineMs);
            _cognitoIdentityLazy = new Lazy<CognitoIdentity>(() => CognitoIdentity.FromJson(runtimeApiHeaders.CognitoIdentityJson));
            _cognitoClientContextLazy = new Lazy<CognitoClientContext>(() => CognitoClientContext.FromJson(runtimeApiHeaders.ClientContextJson));

            // set environment variable so that if the function uses the XRay client it will work correctly
            Environment.SetEnvironmentVariable("_X_AMZN_TRACE_ID", _runtimeApiHeaders.TraceId);
        }

        // TODO If/When Amazon.Lambda.Core is major versioned, add this to ILambdaContext.
        // Until then function code can access it via the _X_AMZN_TRACE_ID environment variable set by LambdaBootstrap.
        public string? TraceId => _runtimeApiHeaders.TraceId;

        public string AwsRequestId => _runtimeApiHeaders.AwsRequestId;

        public IClientContext ClientContext => _cognitoClientContextLazy.Value;

        public string? FunctionName => Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME");

        public string? FunctionVersion => Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_VERSION");

        public ICognitoIdentity Identity => _cognitoIdentityLazy.Value;

        public string? InvokedFunctionArn => _runtimeApiHeaders.InvokedFunctionArn;

        public ILambdaLogger Logger => new LambdaConsoleLogger(_consoleLogger);

        public string? LogGroupName => Environment.GetEnvironmentVariable("AWS_LAMBDA_LOG_GROUP_NAME");

        public string? LogStreamName => Environment.GetEnvironmentVariable("AWS_LAMBDA_LOG_STREAM_NAME");

        public int MemoryLimitInMB => _memoryLimitInMB;

        public TimeSpan RemainingTime => TimeSpan.FromMilliseconds(_deadlineMs - (DateTime.UtcNow - UnixEpoch).TotalMilliseconds);
    }
}
