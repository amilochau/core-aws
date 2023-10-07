using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;
using System.Text.Json;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/GetItemRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// GetItem Request Marshaller
    /// </summary>       
    public class GetItemRequestMarshaller : IMarshaller<IRequest, GetItemRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((GetItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public IRequest Marshall(GetItemRequest publicRequest)
        {
            var serializedRequest = JsonSerializer.Serialize(publicRequest, AwsJsonSerializerContext.Default.GetItemRequest);

            IRequest request = new DefaultRequest(publicRequest, "Amazon.DynamoDBv2")
            {
                HttpMethod = "POST",
                ResourcePath = "/",
                Content = System.Text.Encoding.UTF8.GetBytes(serializedRequest)
            };
            request.Headers["X-Amz-Target"] = "DynamoDB_20120810.GetItem";
            request.Headers["Content-Type"] = "application/x-amz-json-1.0";
            request.Headers[Amazon.Util.HeaderKeys.XAmzApiVersion] = "2012-08-10";

            return request;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static GetItemRequestMarshaller Instance { get; } = new GetItemRequestMarshaller();
    }
}
