using System;

namespace Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Context
{
    /// <summary>
    /// Provides access to Environment Variables set by the Lambda runtime environment.
    /// </summary>
    public class LambdaEnvironment
    {
        internal const string EnvVarServerHostAndPort = "AWS_LAMBDA_RUNTIME_API";
        internal const string EnvVarTraceId = "_X_AMZN_TRACE_ID";

        internal LambdaEnvironment()
        {
            RuntimeServerHostAndPort = Environment.GetEnvironmentVariable(EnvVarServerHostAndPort);
        }

        internal void SetXAmznTraceId(string xAmznTraceId)
        {
            Environment.SetEnvironmentVariable(EnvVarTraceId, xAmznTraceId);
        }

        public string RuntimeServerHostAndPort { get; private set; }
    }
}
