using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Milochau.Core.Aws.Cognito.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for GetItem operation
    /// </summary>  
    public class InitiateAuthResponseUnmarshaller : JsonResponseUnmarshaller
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            return JsonSerializer.Deserialize(context.Stream, InitiateAuthJsonSerializerContext.Default.InitiateAuthResponse)!; // @todo null?
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
        public static InitiateAuthResponseUnmarshaller Instance { get; } = new InitiateAuthResponseUnmarshaller();
    }
}
