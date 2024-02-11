using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Cognito
{
    /// <summary>
    /// Constants used for properties of type AuthFlowType.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<AuthFlowType>))]
    public enum AuthFlowType
    {
        /// <summary>
        /// Constant ADMIN_NO_SRP_AUTH for AuthFlowType
        /// </summary>
        ADMIN_NO_SRP_AUTH,
        /// <summary>
        /// Constant ADMIN_USER_PASSWORD_AUTH for AuthFlowType
        /// </summary>
        ADMIN_USER_PASSWORD_AUTH,
        /// <summary>
        /// Constant CUSTOM_AUTH for AuthFlowType
        /// </summary>
        CUSTOM_AUTH,
        /// <summary>
        /// Constant REFRESH_TOKEN for AuthFlowType
        /// </summary>
        REFRESH_TOKEN,
        /// <summary>
        /// Constant REFRESH_TOKEN_AUTH for AuthFlowType
        /// </summary>
        REFRESH_TOKEN_AUTH,
        /// <summary>
        /// Constant USER_PASSWORD_AUTH for AuthFlowType
        /// </summary>
        USER_PASSWORD_AUTH,
        /// <summary>
        /// Constant USER_SRP_AUTH for AuthFlowType
        /// </summary>
        USER_SRP_AUTH,
    }

    /// <summary>
    /// Constants used for properties of type ChallengeNameType.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ChallengeNameType>))]
    public enum ChallengeNameType
    {
        /// <summary>
        /// Constant ADMIN_NO_SRP_AUTH for ChallengeNameType
        /// </summary>
        ADMIN_NO_SRP_AUTH,
        /// <summary>
        /// Constant CUSTOM_CHALLENGE for ChallengeNameType
        /// </summary>
        CUSTOM_CHALLENGE,
        /// <summary>
        /// Constant DEVICE_PASSWORD_VERIFIER for ChallengeNameType
        /// </summary>
        DEVICE_PASSWORD_VERIFIER,
        /// <summary>
        /// Constant DEVICE_SRP_AUTH for ChallengeNameType
        /// </summary>
        DEVICE_SRP_AUTH,
        /// <summary>
        /// Constant MFA_SETUP for ChallengeNameType
        /// </summary>
        MFA_SETUP,
        /// <summary>
        /// Constant NEW_PASSWORD_REQUIRED for ChallengeNameType
        /// </summary>
        NEW_PASSWORD_REQUIRED,
        /// <summary>
        /// Constant PASSWORD_VERIFIER for ChallengeNameType
        /// </summary>
        PASSWORD_VERIFIER,
        /// <summary>
        /// Constant SELECT_MFA_TYPE for ChallengeNameType
        /// </summary>
        SELECT_MFA_TYPE,
        /// <summary>
        /// Constant SMS_MFA for ChallengeNameType
        /// </summary>
        SMS_MFA,
        /// <summary>
        /// Constant SOFTWARE_TOKEN_MFA for ChallengeNameType
        /// </summary>
        SOFTWARE_TOKEN_MFA,
    }
}
