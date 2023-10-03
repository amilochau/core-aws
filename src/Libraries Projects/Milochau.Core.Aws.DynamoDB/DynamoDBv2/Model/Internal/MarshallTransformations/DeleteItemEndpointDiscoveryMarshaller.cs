using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/DeleteItemEndpointDiscoveryMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Endpoint discovery parameters for DeleteItem operation
    /// </summary>  
    public class DeleteItemEndpointDiscoveryMarshaller : IMarshaller<EndpointDiscoveryDataBase, DeleteItemRequest>, IMarshaller<EndpointDiscoveryDataBase, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the endpoint discovery object.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public EndpointDiscoveryDataBase Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((DeleteItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public EndpointDiscoveryDataBase Marshall(DeleteItemRequest publicRequest)
        {
            var endpointDiscoveryData = new EndpointDiscoveryData(false);

            return endpointDiscoveryData;
        }

        private static DeleteItemEndpointDiscoveryMarshaller _instance = new DeleteItemEndpointDiscoveryMarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static DeleteItemEndpointDiscoveryMarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
