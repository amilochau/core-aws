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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using Amazon.Util;
using Amazon.Runtime.Internal.Util;
using System.Globalization;

namespace Amazon.Runtime.Internal.Transform
{
    /// <summary>
    /// Abstract class for unmarshalling service responses.
    /// </summary>
    public abstract class ResponseUnmarshaller : IResponseUnmarshaller<AmazonWebServiceResponse, UnmarshallerContext>
    {
        public virtual UnmarshallerContext CreateContext(IWebResponseData response, bool readEntireResponse, Stream stream, RequestMetrics metrics, bool isException, IRequestContext requestContext)
        {
            if (response == null)
            {
                throw new AmazonServiceException("The Web Response for a successful request is null!");
            }

            UnmarshallerContext context;

            // If the request is using the flexible checksum feature to verify a response checksum,
            // then both the service package and core will support the ConstructUnmarshallerContext
            // override with 5 parameters
            if (requestContext?.OriginalRequest?.CoreChecksumMode != CoreChecksumResponseBehavior.DISABLED)
            {
                context = ConstructUnmarshallerContext(stream,
                ShouldReadEntireResponse(response, readEntireResponse),
                response,
                isException,
                requestContext);
            }
            else // Otherwise it's okay to skip passing in requestContext, because 
            // it's only currently used when validating a response checksum. This will also
            // preserve existing behavior for a core package version with flexible checksum
            // support alongside an older service package version.
            {
                context = ConstructUnmarshallerContext(stream,
                ShouldReadEntireResponse(response, readEntireResponse),
                response,
                isException);
            }

            return context;
        }

        public virtual bool HasStreamingProperty
        {
            get { return false; }
        }

#region IResponseUnmarshaller<AmazonWebServiceResponse,UnmarshallerContext> Members

        public virtual AmazonServiceException UnmarshallException(UnmarshallerContext input, Exception innerException, HttpStatusCode statusCode)
        {
            throw new NotImplementedException();
        }

#endregion

        public AmazonWebServiceResponse UnmarshallResponse(UnmarshallerContext context)
        {
            var response = this.Unmarshall(context);
            response.ContentLength = context.ResponseData.ContentLength;
            response.HttpStatusCode = context.ResponseData.StatusCode;
            return response;
        }

#region IUnmarshaller<AmazonWebServiceResponse,UnmarshallerContext> Members

        public abstract AmazonWebServiceResponse Unmarshall(UnmarshallerContext input);

#endregion

        protected abstract UnmarshallerContext ConstructUnmarshallerContext(
           Stream responseStream, bool maintainResponseBody, IWebResponseData response, bool isException); 
        
        protected abstract UnmarshallerContext ConstructUnmarshallerContext(
            Stream responseStream, bool maintainResponseBody, IWebResponseData response, bool isException, IRequestContext requestContext);

        protected virtual bool ShouldReadEntireResponse(IWebResponseData response, bool readEntireResponse)
        {
            return readEntireResponse;
        }
    }

    /// <summary>
    /// Class for unmarshalling JSON service responses.
    /// </summary>
    public abstract class JsonResponseUnmarshaller : ResponseUnmarshaller
    {
        public override AmazonWebServiceResponse Unmarshall(UnmarshallerContext input)
        {
            JsonUnmarshallerContext context = input as JsonUnmarshallerContext;
            if (context == null)
                throw new InvalidOperationException("Unsupported UnmarshallerContext");

            string requestId = context.ResponseData.GetHeaderValue(HeaderKeys.RequestIdHeader);
            try
            {
                var response = this.Unmarshall(context);
                response.ResponseMetadata = new ResponseMetadata();
                response.ResponseMetadata.RequestId = requestId;
                return response;
            }
            catch (Exception e)
            {
                throw new AmazonUnmarshallingException(requestId, context.CurrentPath, e, context.ResponseData.StatusCode);
            }
        }
        public override AmazonServiceException UnmarshallException(UnmarshallerContext input, Exception innerException, HttpStatusCode statusCode)
        {
            JsonUnmarshallerContext context = input as JsonUnmarshallerContext;
            if (context == null)
                throw new InvalidOperationException("Unsupported UnmarshallerContext");

            var responseException = this.UnmarshallException(context, innerException, statusCode);
            responseException.RequestId = context.ResponseData.GetHeaderValue(HeaderKeys.RequestIdHeader);
            return responseException;
        }

        public abstract AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext input);

        public abstract AmazonServiceException UnmarshallException(JsonUnmarshallerContext input, Exception innerException, HttpStatusCode statusCode);

        protected override UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData response, bool isException)
        {
            return new JsonUnmarshallerContext(responseStream, maintainResponseBody, response, isException, null);
        }

        protected override UnmarshallerContext ConstructUnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData response, bool isException, IRequestContext requestContext)
        {
            return new JsonUnmarshallerContext(responseStream, maintainResponseBody, response, isException, requestContext);
        }

        protected override bool ShouldReadEntireResponse(IWebResponseData response, bool readEntireResponse)
        {
            return readEntireResponse && response.ContentType != "application/octet-stream";
        }
    }
}
