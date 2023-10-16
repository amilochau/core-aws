using System;
using System.Net;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Class for unmarshalling JSON service responses.
    /// </summary>
    public abstract class JsonResponseUnmarshaller : IResponseUnmarshaller<AmazonWebServiceResponse, JsonUnmarshallerContext>
    {
        public AmazonWebServiceResponse UnmarshallResponse(JsonUnmarshallerContext context)
        {
            var response = Unmarshall(context);
            response.HttpStatusCode = context.ResponseData.StatusCode;
            return response;
        }

        public abstract AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context);

        public abstract AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode);
    }
}
