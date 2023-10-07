using Amazon.Runtime;
using System;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/AmazonDynamoDBDefaultConfiguration.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2
{
    /// <summary>
    /// Configuration for accessing Amazon DynamoDB service
    /// </summary>
    public static class AmazonDynamoDBDefaultConfiguration
    {
        /// <summary>
        /// <p>The STANDARD mode provides the latest recommended default values that should be safe to run in most scenarios</p><p>Note that the default values vended from this mode might change as best practices may evolve. As a result, it is encouraged to perform tests when upgrading the SDK</p>
        /// </summary>
        public static IDefaultConfiguration Standard { get; } = new DefaultConfiguration
        {
            StsRegionalEndpoints = StsRegionalEndpointsValue.Regional,
            S3UsEast1RegionalEndpoint = S3UsEast1RegionalEndpointValue.Regional,
            // 0:00:03.1
            ConnectTimeout = TimeSpan.FromMilliseconds(3100L),
            // 0:00:03.1
            TlsNegotiationTimeout = TimeSpan.FromMilliseconds(3100L),
            HttpRequestTimeout = null
        };
    }
}
