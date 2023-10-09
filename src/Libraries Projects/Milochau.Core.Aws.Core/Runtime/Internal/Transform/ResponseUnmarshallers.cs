using System;
using System.Net;
using System.IO;
using Milochau.Core.Aws.Core.Util;
using Milochau.Core.Aws.Core.Runtime.Pipeline;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Abstract class for unmarshalling service responses.
    /// </summary>
    public abstract class ResponseUnmarshaller : IResponseUnmarshaller<AmazonWebServiceResponse, UnmarshallerContext>
    {
        public virtual UnmarshallerContext CreateContext(IWebResponseData response, bool readEntireResponse, Stream stream, bool isException, IRequestContext requestContext)
        {
            if (response == null)
            {
                throw new AmazonServiceException("The Web Response for a successful request is null!");
            }

            return ConstructUnmarshallerContext(stream,
                ShouldReadEntireResponse(response, readEntireResponse),
                response,
                isException);
        }

        #region IResponseUnmarshaller<AmazonWebServiceResponse,UnmarshallerContext> Members

        public abstract AmazonServiceException UnmarshallException(UnmarshallerContext input, Exception innerException, HttpStatusCode statusCode);

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
            JsonUnmarshallerContext? context = input as JsonUnmarshallerContext;
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
            JsonUnmarshallerContext? context = input as JsonUnmarshallerContext;
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
            return new JsonUnmarshallerContext(responseStream, maintainResponseBody, response, isException);
        }

        protected override bool ShouldReadEntireResponse(IWebResponseData response, bool readEntireResponse)
        {
            return readEntireResponse && response.ContentType != "application/octet-stream";
        }
    }
}
