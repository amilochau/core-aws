using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/BatchWriteItemEndpointDiscoveryMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Endpoint discovery parameters for BatchWriteItem operation
    /// </summary>  
    public class BatchWriteItemEndpointDiscoveryMarshaller : IMarshaller<EndpointDiscoveryDataBase, BatchWriteItemRequest>, IMarshaller<EndpointDiscoveryDataBase, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the endpoint discovery object.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public EndpointDiscoveryDataBase Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((BatchWriteItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public EndpointDiscoveryDataBase Marshall(BatchWriteItemRequest publicRequest)
        {
            var endpointDiscoveryData = new EndpointDiscoveryData(false);

            return endpointDiscoveryData;
        }

        private static BatchWriteItemEndpointDiscoveryMarshaller _instance = new BatchWriteItemEndpointDiscoveryMarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static BatchWriteItemEndpointDiscoveryMarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
