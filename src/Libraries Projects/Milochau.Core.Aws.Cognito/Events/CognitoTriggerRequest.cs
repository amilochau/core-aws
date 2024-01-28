﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Cognito.Events
{
    /// <summary>
    /// https://docs.aws.amazon.com/cognito/latest/developerguide/cognito-user-identity-pools-working-with-aws-lambda-triggers.html#cognito-user-pools-lambda-trigger-syntax-shared
    /// </summary>
    public abstract class CognitoTriggerRequest
    {
        /// <summary>
        /// One or more pairs of user attribute names and values.Each pair is in the form "name": "value".
        /// </summary>
        [JsonPropertyName("userAttributes")]
        public Dictionary<string, string> UserAttributes { get; set; } = new Dictionary<string, string>();
    }
}