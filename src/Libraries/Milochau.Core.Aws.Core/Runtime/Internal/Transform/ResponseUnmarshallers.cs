using System.Net;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Class for unmarshalling JSON service responses.
    /// </summary>
    public abstract class JsonResponseUnmarshaller : IResponseUnmarshaller<AmazonWebServiceResponse, JsonUnmarshallerContext>
    {
        /// <summary>Unmarshall response</summary>
        public AmazonWebServiceResponse UnmarshallResponse(JsonUnmarshallerContext context)
        {
            var response = Unmarshall(context);
            response.HttpStatusCode = context.ResponseData.StatusCode;
            return response;
        }

        /// <summary>Unmarshall response</summary>
        public abstract AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context);

        /// <summary>Unmarshall esception</summary>
        public abstract AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode);
    }
}
