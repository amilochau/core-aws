// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Endpoint.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// An endpoint information details.
    /// </summary>
    public partial class Endpoint
    {
        /// <summary>
        /// Gets and sets the property Address. 
        /// <para>
        /// IP address of the endpoint.
        /// </para>
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Gets and sets the property CachePeriodInMinutes. 
        /// <para>
        /// Endpoint cache time to live (TTL) value.
        /// </para>
        /// </summary>
        public long CachePeriodInMinutes { get; set; }
    }
}
