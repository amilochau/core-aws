using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// The capacity units consumed by an operation. The data returned includes the total
    /// provisioned throughput consumed, along with statistics for the table and any indexes
    /// involved in the operation. <code>ConsumedCapacity</code> is only returned if the request
    /// asked for it. For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ProvisionedThroughputIntro.html">Provisioned
    /// Throughput</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </summary>
    public partial class ConsumedCapacity
    {
        /// <summary>
        /// Gets and sets the property CapacityUnits. 
        /// <para>
        /// The total number of capacity units consumed by the operation.
        /// </para>
        /// </summary>
        public double? CapacityUnits { get; set; }

        /// <summary>
        /// Gets and sets the property GlobalSecondaryIndexes. 
        /// <para>
        /// The amount of throughput consumed on each global index affected by the operation.
        /// </para>
        /// </summary>
        public Dictionary<string, Capacity>? GlobalSecondaryIndexes { get; set; }

        /// <summary>
        /// Gets and sets the property LocalSecondaryIndexes. 
        /// <para>
        /// The amount of throughput consumed on each local index affected by the operation.
        /// </para>
        /// </summary>
        public Dictionary<string, Capacity>? LocalSecondaryIndexes { get; set; }

        /// <summary>
        /// Gets and sets the property ReadCapacityUnits. 
        /// <para>
        /// The total number of read capacity units consumed by the operation.
        /// </para>
        /// </summary>
        public double ReadCapacityUnits { get; set; }

        /// <summary>
        /// Gets and sets the property Table. 
        /// <para>
        /// The amount of throughput consumed on the table affected by the operation.
        /// </para>
        /// </summary>
        public Capacity? Table { get; set; }

        /// <summary>
        /// Gets and sets the property TableName. 
        /// <para>
        /// The name of the table that was affected by the operation.
        /// </para>
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// Gets and sets the property WriteCapacityUnits. 
        /// <para>
        /// The total number of write capacity units consumed by the operation.
        /// </para>
        /// </summary>
        public double WriteCapacityUnits { get; set; }
    }
}