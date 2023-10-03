using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/DescribeEndpointsRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// DescribeEndpoints Request Marshaller
    /// </summary>       
    public class DescribeEndpointsRequestMarshaller : IMarshaller<IRequest, DescribeEndpointsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((DescribeEndpointsRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public IRequest Marshall(DescribeEndpointsRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.DynamoDBv2");
            string target = "DynamoDB_20120810.DescribeEndpoints";
            request.Headers["X-Amz-Target"] = target;
            request.Headers["Content-Type"] = "application/x-amz-json-1.0";
            request.Headers[Amazon.Util.HeaderKeys.XAmzApiVersion] = "2012-08-10";
            request.HttpMethod = "POST";

            request.ResourcePath = "/";
            var content = "{}";
            request.Content = System.Text.Encoding.UTF8.GetBytes(content);

            return request;
        }
        private static DescribeEndpointsRequestMarshaller _instance = new DescribeEndpointsRequestMarshaller();

        internal static DescribeEndpointsRequestMarshaller GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static DescribeEndpointsRequestMarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
