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
using System.Collections.Generic;
using Amazon.Util;

namespace Amazon.Runtime.Internal
{
    /// <summary>
    /// This handler marshalls the request before calling invoking the next handler.
    /// </summary>
    public class Marshaller : PipelineHandler
    {
        /// <summary>
        /// Calls pre invoke logic before calling the next handler 
        /// in the pipeline.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            PreInvoke(executionContext);
            return base.InvokeAsync<T>(executionContext);            
        }

        /// <summary>
        /// Marshalls the request before calling invoking the next handler.
        /// </summary>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        protected static void PreInvoke(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            requestContext.Request = requestContext.Marshaller.Marshall(requestContext.OriginalRequest);
            requestContext.Request.AuthenticationRegion = null;

            var userAgent = $"{requestContext.ClientConfig.UserAgent} " +
                $"{(executionContext.RequestContext.IsAsync ? "ClientAsync" : "ClientSync")}";

            requestContext.Request.Headers[HeaderKeys.UserAgentHeader] = userAgent;

            var method = requestContext.Request.HttpMethod.ToUpperInvariant();
            if (method != "GET" && method != "DELETE" && method != "HEAD")
            {
                if (!requestContext.Request.Headers.ContainsKey(HeaderKeys.ContentTypeHeader))
                {
                    if (requestContext.Request.UseQueryString)
                        requestContext.Request.Headers[HeaderKeys.ContentTypeHeader] = "application/x-amz-json-1.0";
                    else
                        requestContext.Request.Headers[HeaderKeys.ContentTypeHeader] = AWSSDKUtils.UrlEncodedContent;
                }
            }

            SetRecursionDetectionHeader(requestContext.Request.Headers);
        }

        /// <summary>
        /// Sets the X-Amzn-Trace-Id header for recursion detection within Lambda workloads.
        /// </summary>
        /// <param name="headers">Current request headers before marshalling.</param>
        private static void SetRecursionDetectionHeader(IDictionary<string, string> headers)
        {
            if (!headers.ContainsKey(HeaderKeys.XAmznTraceIdHeader))
            {                
                var lambdaFunctionName = Environment.GetEnvironmentVariable(EnvironmentVariables.AWS_LAMBDA_FUNCTION_NAME);
                var amznTraceId = Environment.GetEnvironmentVariable(EnvironmentVariables._X_AMZN_TRACE_ID);

                if (!string.IsNullOrEmpty(lambdaFunctionName) && !string.IsNullOrEmpty(amznTraceId))
                {
                    headers[HeaderKeys.XAmznTraceIdHeader] = AWSSDKUtils.EncodeTraceIdHeaderValue(amznTraceId);
                }
            }
        }
    }
}
