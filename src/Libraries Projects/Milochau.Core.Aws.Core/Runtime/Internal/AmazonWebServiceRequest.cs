﻿/*
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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Amazon.Runtime
{
    public abstract partial class AmazonWebServiceRequest
    {
        /// <summary>
        /// Checksum algorithms that are supported for validating the integrity of this request's response
        /// </summary>
        protected internal virtual ReadOnlyCollection<CoreChecksumAlgorithm> ChecksumResponseAlgorithms => new List<CoreChecksumAlgorithm>(0).AsReadOnly();
    }
}
