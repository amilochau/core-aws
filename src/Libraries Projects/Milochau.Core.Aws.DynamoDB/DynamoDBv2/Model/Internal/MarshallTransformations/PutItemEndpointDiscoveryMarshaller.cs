using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/PutItemEndpointDiscoveryMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Endpoint discovery parameters for PutItem operation
    /// </summary>  
    public class PutItemEndpointDiscoveryMarshaller : IMarshaller<EndpointDiscoveryDataBase, PutItemRequest>, IMarshaller<EndpointDiscoveryDataBase, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the endpoint discovery object.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public EndpointDiscoveryDataBase Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((PutItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public EndpointDiscoveryDataBase Marshall(PutItemRequest publicRequest)
        {
            var endpointDiscoveryData = new EndpointDiscoveryData(false);

            return endpointDiscoveryData;
        }

        private static PutItemEndpointDiscoveryMarshaller _instance = new PutItemEndpointDiscoveryMarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static PutItemEndpointDiscoveryMarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
