using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.Core.References
{
    public static class EnvironmentVariables
    {
        public const string Key_DaemonAddress = "AWS_XRAY_DAEMON_ADDRESS";
        public const string Key_TraceId = "_X_AMZN_TRACE_ID";
        public const string Key_AccessKeyId = "AWS_ACCESS_KEY_ID";
        public const string Key_SecretAccessKey = "AWS_SECRET_ACCESS_KEY";
        public const string Key_SessionToken = "AWS_SESSION_TOKEN";
        public const string Key_LambdaRuntimeApi = "AWS_LAMBDA_RUNTIME_API";
        public const string Key_Region = "AWS_REGION";
        public const string Key_DefaultRegion = "AWS_DEFAULT_REGION";
        public const string Key_XRayContextMissing = "AWS_XRAY_CONTEXT_MISSING";

        private static IDictionary Variables { get; } = Environment.GetEnvironmentVariables();

        public static string? GetEnvironmentVariable(string key)
        {
            TryGetEnvironmentVariable(key, out var value);
            return value;
        }

        public static void SetEnvironmentVariable(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value);
        }

        public static bool TryGetEnvironmentVariable(string key, [NotNullWhen(true)] out string? value)
        {
            if (!Variables.Contains(key))
            {
                value = null;
                return false;
            }
            value = Variables[key].ToString();
            return true;
        }

        /// <summary>
        /// Gets the name of the AWS region.
        /// </summary>
        public static string RegionName { get; } = GetEnvironmentVariable(Key_Region);

        /// <summary>
        /// Gets the AccessKey property for the current credentials.
        /// </summary>
        public static string AccessKey { get; } = GetEnvironmentVariable(Key_AccessKeyId);

        /// <summary>
        /// Gets the SecretKey property for the current credentials.
        /// </summary>
        public static string SecretKey { get; } = GetEnvironmentVariable(Key_SecretAccessKey);

        /// <summary>
        /// Gets the Token property for the current credentials.
        /// </summary>
        public static string Token { get; } = GetEnvironmentVariable(Key_SessionToken);

        /// <summary>
        /// Gets the UseToken property for the current credentials.
        /// Specifies if Token property is non-empty.
        /// </summary>
        public static bool UseToken { get; } = !string.IsNullOrEmpty(Token);

        public static string? RuntimeServerHostAndPort { get; } = GetEnvironmentVariable(Key_LambdaRuntimeApi);
    }
}
