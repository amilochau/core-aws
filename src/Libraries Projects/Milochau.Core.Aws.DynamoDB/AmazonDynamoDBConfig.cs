using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Util.Internal;
using Milochau.Core.Aws.DynamoDB.Internal;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>
    /// Configuration for accessing Amazon DynamoDB service
    /// </summary>
    public partial class AmazonDynamoDBConfig : ClientConfig
    {
        private static readonly string UserAgentString =
            InternalSDKUtils.BuildUserAgentString("3.7.202.4");

        private readonly string _userAgent = UserAgentString;

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
        public override string RegionEndpointServiceName => "dynamodb";

        /// <summary>
        /// Gets the value of UserAgent property.
        /// </summary>
        public override string UserAgent => _userAgent;
    }
}
