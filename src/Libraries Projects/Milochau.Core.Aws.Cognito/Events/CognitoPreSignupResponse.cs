﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/user-pool-lambda-pre-sign-up.html
    /// </summary>
    public class CognitoPreSignupResponse : CognitoTriggerResponse
    {
        /// <summary>
        /// Set to true to auto-confirm the user, or false otherwise.
        /// </summary>
        [JsonPropertyName("autoConfirmUser")]
        public bool AutoConfirmUser { get; set; }

        /// <summary>
        /// Set to true to set as verified the email of a user who is signing up, or false otherwise. If autoVerifyEmail is set to true, the email attribute must have a valid, non-null value. Otherwise an error will occur and the user will not be able to complete sign-up.
        /// </summary>
        [JsonPropertyName("autoVerifyPhone")]
        public bool AutoVerifyPhone { get; set; }

        /// <summary>
        /// Set to true to set as verified the phone number of a user who is signing up, or false otherwise. If autoVerifyPhone is set to true, the phone_number attribute must have a valid, non-null value. Otherwise an error will occur and the user will not be able to complete sign-up.
        /// </summary>
        [JsonPropertyName("autoVerifyEmail")]
        public bool AutoVerifyEmail { get; set; }
    }
}
