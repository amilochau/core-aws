using Amazon.Runtime.Internal;
using Amazon.Runtime;
using Amazon.Util.Internal;
using Milochau.Core.Aws.DynamoDB.DynamoDBv2.Internal;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/AmazonDynamoDBConfig.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2
{
    /// <summary>
    /// Configuration for accessing Amazon DynamoDB service
    /// </summary>
    public partial class AmazonDynamoDBConfig : ClientConfig
    {
        private static readonly string UserAgentString =
            InternalSDKUtils.BuildUserAgentString("3.7.202.4");

        private string _userAgent = UserAgentString;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AmazonDynamoDBConfig()
            : base()
        {
            this.AuthenticationServiceName = "dynamodb";
            this.MaxErrorRetry = 10;
            this.EndpointProvider = new AmazonDynamoDBEndpointProvider();
        }

        /// <summary>
        /// The constant used to lookup in the region hash the endpoint.
        /// </summary>
        public override string RegionEndpointServiceName
        {
            get
            {
                return "dynamodb";
            }
        }

        /// <summary>
        /// Gets the value of UserAgent property.
        /// </summary>
        public override string UserAgent
        {
            get
            {
                return _userAgent;
            }
        }
    }
}
