using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/UpdateItemEndpointDiscoveryMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Endpoint discovery parameters for UpdateItem operation
    /// </summary>  
    public class UpdateItemEndpointDiscoveryMarshaller : IMarshaller<EndpointDiscoveryDataBase, UpdateItemRequest>, IMarshaller<EndpointDiscoveryDataBase, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the endpoint discovery object.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public EndpointDiscoveryDataBase Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((UpdateItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public EndpointDiscoveryDataBase Marshall(UpdateItemRequest publicRequest)
        {
            var endpointDiscoveryData = new EndpointDiscoveryData(false);

            return endpointDiscoveryData;
        }

        private static UpdateItemEndpointDiscoveryMarshaller _instance = new UpdateItemEndpointDiscoveryMarshaller();

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static UpdateItemEndpointDiscoveryMarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
