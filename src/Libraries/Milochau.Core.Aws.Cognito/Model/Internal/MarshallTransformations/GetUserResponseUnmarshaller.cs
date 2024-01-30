using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime;
using System.Net;
using System.Text.Json;

namespace Milochau.Core.Aws.Cognito.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for GetUser operation
    /// </summary>  
    public class GetUserResponseUnmarshaller : JsonResponseUnmarshaller
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            return JsonSerializer.Deserialize(context.Stream, GetUserJsonSerializerContext.Default.GetUserResponse)!; // @todo null?
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>  
        public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.Unmarshall(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonCognitoIdentityProviderException(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static GetUserResponseUnmarshaller Instance { get; } = new GetUserResponseUnmarshaller();
    }
}
