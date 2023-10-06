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
using System.Text;
using System.IO;
using Amazon.Util;

namespace Amazon.Lambda.Model
{
    public partial class InvokeRequest : AmazonLambdaRequest
    {
        /// <summary>
        /// Gets and sets the property Payload. When this property is set the PayloadStream
        /// property is also set with a MemoryStream containing the contents of Payload.
        /// <para>
        /// JSON that you want to provide to your cloud function as input.
        /// </para>
        /// </summary>
        public string Payload
        {
            get
            {
                string content = null;
                if (this.PayloadStream != null)
                {
                    content = new StreamReader(this.PayloadStream).ReadToEnd();
                    this.PayloadStream.Position = 0;
                }
                return content;
            }
            set
            {
                if (value == null)
                    this.PayloadStream = null;
                else
                    this.PayloadStream = AWSSDKUtils.GenerateMemoryStreamFromString(value);
            }
        }
    }
}