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
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Util;

namespace Amazon.Runtime.Internal.Transform
{

    internal class InternalException
    {
        [JsonPropertyName("__type")]
        public string? Type { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }

    /// <summary>JSON serialization context</summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(InternalException))]
    internal partial class InternalExceptionJsonSerializerContext : JsonSerializerContext
    {
    }

    /// <summary>
    ///    First-pass unmarshaller for all errors
    /// </summary>
    public class JsonErrorResponseUnmarshaller : IUnmarshaller<ErrorResponse, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Build an ErrorResponse from json 
        /// </summary>
        /// <param name="context">The json parsing context. 
        /// Usually an <c>Amazon.Runtime.Internal.JsonUnmarshallerContext</c>.</param>
        /// <returns>An <c>ErrorResponse</c> object.</returns>
        public ErrorResponse Unmarshall(JsonUnmarshallerContext context)
        {
            string requestId = null;
            GetValuesFromJsonIfPossible(context, out InternalException internalException);

            // If an error code was not found, check for the x-amzn-ErrorType header. 
            // This header is returned by rest-json services.
            if (string.IsNullOrEmpty(internalException.Type) &&
                context.ResponseData.IsHeaderPresent(HeaderKeys.XAmzErrorType))
            {
                var errorType = context.ResponseData.GetHeaderValue(HeaderKeys.XAmzErrorType);
                if (!string.IsNullOrEmpty(errorType))
                {
                    // The error type can contain additional information, with ":" as a delimiter
                    // We are only interested in the initial part which is the error type
                    var index = errorType.IndexOf(":", StringComparison.Ordinal);
                    if(index != -1)
                    {
                        errorType = errorType.Substring(0, index);
                    }
                    internalException.Type = errorType;
                }
            }

            // Check for the x-amzn-error-message header. This header is returned by rest-json services.
            // If the header is present it is preferred over any value provided in the response body.
            if (context.ResponseData.IsHeaderPresent(HeaderKeys.XAmznErrorMessage))
            {
                var errorMessage = context.ResponseData.GetHeaderValue(HeaderKeys.XAmznErrorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    internalException.Message = errorMessage;
            }

            // if both "__type" and HeaderKeys.XAmzErrorType were not specified, use "code" as type
            // this impacts Glacier
            if (string.IsNullOrEmpty(internalException.Type) && !string.IsNullOrEmpty(internalException.Code))
            {
                internalException.Type = internalException.Code;
            }

            // strip extra data from type, leaving only the exception type name
            internalException.Type = internalException?.Type.Substring(internalException.Type.LastIndexOf("#", StringComparison.Ordinal) + 1);

            // if no message was found create a generic message
            if (string.IsNullOrEmpty(internalException.Message))
            {
                if (string.IsNullOrEmpty(internalException.Type))
                {
                    if (string.IsNullOrEmpty(context.ResponseBody))
                        internalException.Message = "The service returned an error. See inner exception for details.";
                    else
                        internalException.Message = "The service returned an error with HTTP Body: " + context.ResponseBody;
                }
                else
                {
                    if (string.IsNullOrEmpty(context.ResponseBody))
                        internalException.Message = "The service returned an error with Error Code " + internalException.Type + ".";
                    else
                        internalException.Message = "The service returned an error with Error Code " + internalException.Type + " and HTTP Body: " + context.ResponseBody;
                }
            }

            // Check for the x-amzn-RequestId header. This header is returned by rest-json services.
            // If the header is present it is preferred over any value provided in the response body.
            if (context.ResponseData.IsHeaderPresent(HeaderKeys.RequestIdHeader))
            {
                requestId = context.ResponseData.GetHeaderValue(HeaderKeys.RequestIdHeader);
            }

            return new ErrorResponse
            {
                Code = internalException.Type,
                Message = internalException.Message,
                // type is not applicable to JSON services, setting to Unknown
                Type = ErrorType.Unknown,
                RequestId = requestId
            };
        }

        private static void GetValuesFromJsonIfPossible(JsonUnmarshallerContext context, out InternalException internalException)
        {
            internalException = JsonSerializer.Deserialize(context.Stream, InternalExceptionJsonSerializerContext.Default.InternalException);
        }

        private static JsonErrorResponseUnmarshaller instance;

        /// <summary>
        /// Return an instance of JsonErrorResponseUnmarshaller.
        /// </summary>
        /// <returns></returns>
        public static JsonErrorResponseUnmarshaller GetInstance()
        {
            if (instance == null)
                instance = new JsonErrorResponseUnmarshaller();

            return instance;
        }
    }
}
