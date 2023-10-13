using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.DynamoDB.Internal;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>
    /// Configuration for accessing Amazon DynamoDB service
    /// </summary>
    public partial class AmazonDynamoDBConfig : ClientConfig
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AmazonDynamoDBConfig()
        {
            AuthenticationServiceName = "dynamodb";
            MaxErrorRetry = 10;
            EndpointProvider = new AmazonDynamoDBEndpointProvider();
        }
    }
}
