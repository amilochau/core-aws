// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Capacity.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// Represents the amount of provisioned throughput capacity consumed on a table or an
    /// index.
    /// </summary>
    public partial class Capacity
    {
        /// <summary>
        /// Gets and sets the property CapacityUnits. 
        /// <para>
        /// The total number of capacity units consumed on a table or an index.
        /// </para>
        /// </summary>
        public double CapacityUnits { get; set; }

        /// <summary>
        /// Gets and sets the property ReadCapacityUnits. 
        /// <para>
        /// The total number of read capacity units consumed on a table or an index.
        /// </para>
        /// </summary>
        public double ReadCapacityUnits { get; set; }

        /// <summary>
        /// Gets and sets the property WriteCapacityUnits. 
        /// <para>
        /// The total number of write capacity units consumed on a table or an index.
        /// </para>
        /// </summary>
        public double WriteCapacityUnits { get; set; }
    }
}