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

/*
 * Do not modify this file. This file is generated from the lambda-2015-03-31.normal.json service model.
 */
using System;
using System.IO;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.Lambda.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for Invoke operation
    /// </summary>  
    public class InvokeResponseUnmarshaller : JsonResponseUnmarshaller
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            InvokeResponse response = new InvokeResponse();

            var ms = new MemoryStream();
            Amazon.Util.AWSSDKUtils.CopyStream(context.Stream, ms);
            ms.Seek(0, SeekOrigin.Begin);
            response.Payload = ms;
            if (context.ResponseData.IsHeaderPresent("X-Amz-Executed-Version"))
                response.ExecutedVersion = context.ResponseData.GetHeaderValue("X-Amz-Executed-Version");
            if (context.ResponseData.IsHeaderPresent("X-Amz-Function-Error"))
                response.FunctionError = context.ResponseData.GetHeaderValue("X-Amz-Function-Error");
            if (context.ResponseData.IsHeaderPresent("X-Amz-Log-Result"))
                response.LogResult = context.ResponseData.GetHeaderValue("X-Amz-Log-Result");
            response.StatusCode = (int)context.ResponseData.StatusCode;

            return response;
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>  
        /// <param name="context"></param>
        /// <param name="innerException"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
            errorResponse.InnerException = innerException;
            errorResponse.StatusCode = statusCode;

            var responseBodyBytes = context.GetResponseBodyBytes();

            using (var streamCopy = new MemoryStream(responseBodyBytes))
            using (var contextCopy = new JsonUnmarshallerContext(streamCopy, false, null))
            {
                if (errorResponse.Code != null)
                {
                    return GenericExceptionUnmarshaller.Instance.Unmarshall(contextCopy, errorResponse);
                }
            }
            return new AmazonLambdaException(errorResponse.Message, errorResponse.InnerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }

        private static InvokeResponseUnmarshaller _instance = new InvokeResponseUnmarshaller();        

        internal static InvokeResponseUnmarshaller GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static InvokeResponseUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}