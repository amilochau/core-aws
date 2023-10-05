/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */
using System;

namespace Amazon.Runtime
{
    /// <summary>
    /// Uses aws credentials stored in environment variables to construct the credentials object.
    /// AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY are used for the access key id and secret key. 
    /// If the variable AWS_SESSION_TOKEN exists then it will be used to create temporary session 
    /// credentials.
    /// </summary>
    public class EnvironmentVariablesAWSCredentials : AWSCredentials
    {
        public const string ENVIRONMENT_VARIABLE_ACCESSKEY = "AWS_ACCESS_KEY_ID";
        public const string ENVIRONMENT_VARIABLE_SECRETKEY = "AWS_SECRET_ACCESS_KEY";
        public const string ENVIRONMENT_VARIABLE_SESSION_TOKEN = "AWS_SESSION_TOKEN";

        #region Public constructors

        /// <summary>
        /// Constructs an instance of EnvironmentVariablesAWSCredentials. If no credentials are found in 
        /// the environment variables then an InvalidOperationException is thrown.
        /// </summary>
        public EnvironmentVariablesAWSCredentials()
        {
            // We need to do an initial fetch to validate that we can use environment variables to get the credentials.
            FetchCredentials();
        }

        #endregion

        /// <summary>
        /// Creates immutable credentials from environment variables.
        /// </summary>
        /// <returns></returns>
        public static ImmutableCredentials FetchCredentials()
        {
            string accessKeyId = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_ACCESSKEY);
            string secretKey = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_SECRETKEY);
            string sessionToken = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_SESSION_TOKEN);

            return new ImmutableCredentials(accessKeyId, secretKey, sessionToken);
        }

        /// <summary>
        /// Returns an instance of ImmutableCredentials for this instance
        /// </summary>
        /// <returns></returns>
        public override ImmutableCredentials GetCredentials()
        {
            return FetchCredentials();
        }
    }
}
