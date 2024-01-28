using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito
{
    // @todo To be improved with .NET 8 - see https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation-modes?pivots=dotnet-8-0#serialize-enum-fields-as-strings

    /// <summary>
    /// Constants used for properties of type AuthFlowType.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<AuthFlowType>))]
    public static class AuthFlowType
    {
        /// <summary>
        /// Constant ADMIN_NO_SRP_AUTH for AuthFlowType
        /// </summary>
        public const string ADMIN_NO_SRP_AUTH = "ADMIN_NO_SRP_AUTH";
        /// <summary>
        /// Constant ADMIN_USER_PASSWORD_AUTH for AuthFlowType
        /// </summary>
        public const string ADMIN_USER_PASSWORD_AUTH = "ADMIN_USER_PASSWORD_AUTH";
        /// <summary>
        /// Constant CUSTOM_AUTH for AuthFlowType
        /// </summary>
        public const string CUSTOM_AUTH = "CUSTOM_AUTH";
        /// <summary>
        /// Constant REFRESH_TOKEN for AuthFlowType
        /// </summary>
        public const string REFRESH_TOKEN = "REFRESH_TOKEN";
        /// <summary>
        /// Constant REFRESH_TOKEN_AUTH for AuthFlowType
        /// </summary>
        public const string REFRESH_TOKEN_AUTH = "REFRESH_TOKEN_AUTH";
        /// <summary>
        /// Constant USER_PASSWORD_AUTH for AuthFlowType
        /// </summary>
        public const string USER_PASSWORD_AUTH = "USER_PASSWORD_AUTH";
        /// <summary>
        /// Constant USER_SRP_AUTH for AuthFlowType
        /// </summary>
        public const string USER_SRP_AUTH = "USER_SRP_AUTH";
    }

    /// <summary>
    /// Constants used for properties of type ChallengeNameType.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ChallengeNameType>))]
    public static class ChallengeNameType
    {
        /// <summary>
        /// Constant ADMIN_NO_SRP_AUTH for ChallengeNameType
        /// </summary>
        public const string ADMIN_NO_SRP_AUTH = "ADMIN_NO_SRP_AUTH";
        /// <summary>
        /// Constant CUSTOM_CHALLENGE for ChallengeNameType
        /// </summary>
        public const string CUSTOM_CHALLENGE = "CUSTOM_CHALLENGE";
        /// <summary>
        /// Constant DEVICE_PASSWORD_VERIFIER for ChallengeNameType
        /// </summary>
        public const string DEVICE_PASSWORD_VERIFIER = "DEVICE_PASSWORD_VERIFIER";
        /// <summary>
        /// Constant DEVICE_SRP_AUTH for ChallengeNameType
        /// </summary>
        public const string DEVICE_SRP_AUTH = "DEVICE_SRP_AUTH";
        /// <summary>
        /// Constant MFA_SETUP for ChallengeNameType
        /// </summary>
        public const string MFA_SETUP = "MFA_SETUP";
        /// <summary>
        /// Constant NEW_PASSWORD_REQUIRED for ChallengeNameType
        /// </summary>
        public const string NEW_PASSWORD_REQUIRED = "NEW_PASSWORD_REQUIRED";
        /// <summary>
        /// Constant PASSWORD_VERIFIER for ChallengeNameType
        /// </summary>
        public const string PASSWORD_VERIFIER = "PASSWORD_VERIFIER";
        /// <summary>
        /// Constant SELECT_MFA_TYPE for ChallengeNameType
        /// </summary>
        public const string SELECT_MFA_TYPE = "SELECT_MFA_TYPE";
        /// <summary>
        /// Constant SMS_MFA for ChallengeNameType
        /// </summary>
        public const string SMS_MFA = "SMS_MFA";
        /// <summary>
        /// Constant SOFTWARE_TOKEN_MFA for ChallengeNameType
        /// </summary>
        public const string SOFTWARE_TOKEN_MFA = "SOFTWARE_TOKEN_MFA";
    }
}
