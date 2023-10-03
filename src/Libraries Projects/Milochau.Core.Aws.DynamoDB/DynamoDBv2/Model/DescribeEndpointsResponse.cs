using Amazon.Runtime;
using System.Collections.Generic;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/DescribeEndpointsResponse.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// This is the response object from the DescribeEndpoints operation.
    /// </summary>
    public partial class DescribeEndpointsResponse : AmazonWebServiceResponse
    {
        /// <summary>
        /// Gets and sets the property Endpoints. 
        /// <para>
        /// List of endpoints.
        /// </para>
        /// </summary>
        public List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
    }
}
