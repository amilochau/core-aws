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

using System;

namespace Amazon.Util.Internal
{
    /// <summary>
    /// Wrapper class which invokes the static method 
    /// public static string GetEnvironmentVariable(string variable)
    /// underneath. This class is added as a property on the singleton class
    /// EnvironmentVariableSource. This change was done for testability.
    /// </summary>
    public sealed class EnvironmentVariableRetriever : IEnvironmentVariableRetriever
    {
        public string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
