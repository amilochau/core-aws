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
    /// Custom exception type thrown when authentication for a user fails due to
    /// invalid credentials.
    /// </summary>
    public class FederatedAuthenticationFailureException : Exception
    {
        /// <summary>
        /// Initializes a new exception instance.
        /// </summary>
        /// <param name="msg"></param>
        public FederatedAuthenticationFailureException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Initializes a new exception instance.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="inner"></param>
        public FederatedAuthenticationFailureException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}
