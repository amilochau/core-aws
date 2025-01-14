using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.Core.References
{
    /// <summary>In-memory cache for environment variables</summary>
    public static class EnvironmentVariables
    {
        internal const string Key_TraceId = "_X_AMZN_TRACE_ID";
        internal const string Key_AccessKeyId = "AWS_ACCESS_KEY_ID";
        internal const string Key_DefaultRegion = "AWS_DEFAULT_REGION";
        internal const string Key_LambdaFunctionMemorySize = "AWS_LAMBDA_FUNCTION_MEMORY_SIZE";
        internal const string Key_LambdaFunctionName = "AWS_LAMBDA_FUNCTION_NAME";
        internal const string Key_LambdaLogLevel = "AWS_LAMBDA_LOG_LEVEL";
        internal const string Key_LambdaRuntimeApi = "AWS_LAMBDA_RUNTIME_API";
        internal const string Key_Region = "AWS_REGION";
        internal const string Key_SecretAccessKey = "AWS_SECRET_ACCESS_KEY";
        internal const string Key_SessionToken = "AWS_SESSION_TOKEN";
        internal const string Key_XRayContextMissing = "AWS_XRAY_CONTEXT_MISSING";
        internal const string Key_DaemonAddress = "AWS_XRAY_DAEMON_ADDRESS";
        internal const string Key_ConventionPrefix = "CONVENTION__PREFIX";
        internal const string Key_ConventionOrganization = "CONVENTION__ORGANIZATION";
        internal const string Key_ConventionApplication = "CONVENTION__APPLICATION";
        internal const string Key_ConventionHost = "CONVENTION__HOST";
        internal const string Key_AccountId = "ACCOUNT_ID";
        internal const string Key_RoleArn = "AWS_ROLE_ARN";

        private static readonly IDictionary variables = Environment.GetEnvironmentVariables();

        /// <summary>Get environment variable</summary>
        public static string? GetEnvironmentVariable(string key, bool forceRead = false)
        {
            if (forceRead)
            {
                return Environment.GetEnvironmentVariable(key);
            }
            else
            {
                TryGetEnvironmentVariable(key, out var value);
                return value;
            }
        }

        /// <summary>Try get environment variable</summary>
        public static bool TryGetEnvironmentVariable(string key, [NotNullWhen(true)] out string? value)
        {
            if (!variables.Contains(key))
            {
                value = null;
                return false;
            }
            else
            {
                value = variables[key]!.ToString();
                return value != null;
            }
        }

        /// <summary>REgion name</summary>
        public static string RegionName { get; } = GetEnvironmentVariable(Key_Region)!;

        /// <summary>Access key</summary>
        public static string AccessKey { get; } = GetEnvironmentVariable(Key_AccessKeyId)!;

        /// <summary>Secret key</summary>
        public static string SecretKey { get; } = GetEnvironmentVariable(Key_SecretAccessKey)!;

        /// <summary>Session token</summary>
        public static string? Token { get; } = GetEnvironmentVariable(Key_SessionToken);

        /// <summary>Convention - Prefix</summary>
        public static string ConventionPrefix { get; } = GetEnvironmentVariable(Key_ConventionPrefix)!;

        /// <summary>Convention - Organization</summary>
        public static string ConventionOrganization { get; } = GetEnvironmentVariable(Key_ConventionOrganization)!;

        /// <summary>Convention - Application</summary>
        public static string ConventionApplication{ get; } = GetEnvironmentVariable(Key_ConventionApplication)!;

        /// <summary>Convention - Host</summary>
        public static string ConventionHost { get; } = GetEnvironmentVariable(Key_ConventionHost)!;

        /// <summary>Account id</summary>
        public static string AccountId { get; } = GetEnvironmentVariable(Key_AccountId)!;

        /// <summary>Trace id</summary>
        public static string? TraceId
        {
            get => GetEnvironmentVariable(Key_TraceId)!;
            internal set => SetEnvironmentVariable(Key_TraceId, value);
        }

        /// <summary>Request id</summary>
        public static string? RequestId
        {
            get => GetEnvironmentVariable(Key_TraceId)!;
            internal set => SetEnvironmentVariable(Key_TraceId, value);
        }

        /// <summary>XRay Daemon address</summary>
        public static string? DaemonAddress { get; } = GetEnvironmentVariable(Key_DaemonAddress);

        /// <summary>Role ARN</summary>
        public static string RoleArn { get; } = GetEnvironmentVariable(Key_RoleArn)!;

        /// <summary>Lambda Function memory size, in MB</summary>
        internal static string FunctionMemorySize { get; } = GetEnvironmentVariable(Key_LambdaFunctionMemorySize)!;

        internal static string FunctionName { get; } = GetEnvironmentVariable(Key_LambdaFunctionName)!;

        /// <summary>Lambda log level</summary>
        internal static string LogLevel { get; } = GetEnvironmentVariable(Key_LambdaLogLevel)!;

        internal static string? RuntimeServerHostAndPort { get; } = GetEnvironmentVariable(Key_LambdaRuntimeApi);


        private static void SetEnvironmentVariable(string key, string? value)
        {
            // We don't really set the environment variable
            if (variables.Contains(key))
            {
                variables[key] = value;
            }
            else
            {
                variables.Add(key, value);
            }
        }
    }
}
