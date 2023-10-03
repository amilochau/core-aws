using Amazon.Runtime.Internal;
using System.Collections.Generic;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Internal/AmazonDynamoDBMetadata.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Internal
{
    /// <summary>
    /// Service metadata for  Amazon DynamoDB service
    /// </summary>
    public partial class AmazonDynamoDBMetadata : IServiceMetadata
    {
        /// <summary>
        /// Gets the value of the Service Id.
        /// </summary>
        public string ServiceId
        {
            get
            {
                return "DynamoDB";
            }
        }

        /// <summary>
        /// Gets the dictionary that gives mapping of renamed operations
        /// </summary>
        public IDictionary<string, string> OperationNameMapping
        {
            get
            {
                return new Dictionary<string, string>(0)
                {
                };
            }
        }
    }
}
