using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Milochau.Core.Aws.Core.References
{
    /// <summary>In-memory cache for environment variables</summary>
    public static class EnvironmentVariables
    {
        internal const string Key_DaemonAddress = "AWS_XRAY_DAEMON_ADDRESS";
        internal const string Key_TraceId = "_X_AMZN_TRACE_ID";
        internal const string Key_AccessKeyId = "AWS_ACCESS_KEY_ID";
        internal const string Key_SecretAccessKey = "AWS_SECRET_ACCESS_KEY";
        internal const string Key_SessionToken = "AWS_SESSION_TOKEN";
        internal const string Key_LambdaRuntimeApi = "AWS_LAMBDA_RUNTIME_API";
        internal const string Key_Region = "AWS_REGION";
        internal const string Key_DefaultRegion = "AWS_DEFAULT_REGION";
        internal const string Key_XRayContextMissing = "AWS_XRAY_CONTEXT_MISSING";
        internal const string Key_ConventionPrefix = "CONVENTION__PREFIX";
        internal const string Key_ConventionOrganization = "CONVENTION__ORGANIZATION";
        internal const string Key_ConventionApplication = "CONVENTION__APPLICATION";
        internal const string Key_ConventionHost = "CONVENTION__HOST";
        internal const string Key_AccountId = "ACCOUNT_ID";

        private static IDictionary Variables { get; } = Environment.GetEnvironmentVariables();

        /// <summary>Get environment variable</summary>
        public static string? GetEnvironmentVariable(string key)
        {
            TryGetEnvironmentVariable(key, out var value);
            return value;
        }

        /// <summary>Set environment variable</summary>
        public static void SetEnvironmentVariable(string key, string value)
        {
            // We don't really set the environment variable
            if (Variables.Contains(key))
            {
                Variables[key] = value;
            }
            else
            {
                Variables.Add(key, value);
            }
        }

        /// <summary>Try get environment variable</summary>
        public static bool TryGetEnvironmentVariable(string key, [NotNullWhen(true)] out string? value)
        {
            if (!Variables.Contains(key))
            {
                value = null;
                return false;
            }
            else
            {
                value = Variables[key]!.ToString();
                return value != null;
            }
        }

        /// <summary>
        /// Gets the name of the AWS region.
        /// </summary>
        public static string RegionName { get; } = GetEnvironmentVariable(Key_Region)!;

        /// <summary>
        /// Gets the AccessKey property for the current credentials.
        /// </summary>
        public static string AccessKey { get; } = GetEnvironmentVariable(Key_AccessKeyId)!;

        /// <summary>
        /// Gets the SecretKey property for the current credentials.
        /// </summary>
        public static string SecretKey { get; } = GetEnvironmentVariable(Key_SecretAccessKey)!;

        /// <summary>
        /// Gets the Token property for the current credentials.
        /// </summary>
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

        internal static string? RuntimeServerHostAndPort { get; } = GetEnvironmentVariable(Key_LambdaRuntimeApi);
    }
}
