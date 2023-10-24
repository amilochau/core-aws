using Milochau.Core.Aws.Core.Runtime;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>
    /// Configuration for accessing Amazon DynamoDB service
    /// </summary>
    public class AmazonDynamoDBConfig : ClientConfig
    {
        /// <summary>Constructor</summary>
        public AmazonDynamoDBConfig()
        {
            AuthenticationServiceName = "dynamodb";
            MonitoringServiceName = "DynamoDBv2";
        }
    }
}
