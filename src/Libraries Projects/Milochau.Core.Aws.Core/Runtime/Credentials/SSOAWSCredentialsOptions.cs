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
using System.Globalization;
using System.Net;

namespace Amazon.Runtime
{
    public class SsoVerificationArguments
    {
    }

    public class SSOAWSCredentialsOptions
    {
        /// <summary>
        /// Required - Name of the application or system used during SSO client registration.
        /// A timestamp indicating when the client was registered will be appended to requests made to the SSOOIDC service.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// A callback that is used to initiate the SSO Login flow with the user.
        /// </summary>
        public Action<SsoVerificationArguments> SsoVerificationCallback { get; set; }
    }
}
