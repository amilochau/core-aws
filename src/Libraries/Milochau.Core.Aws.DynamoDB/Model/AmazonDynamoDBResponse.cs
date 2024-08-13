using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Base class for DynamoDB operation responses.
    /// </summary>
    public class AmazonDynamoDBResponse : AmazonWebServiceResponse
    {
        /// <summary>
        /// Consumed capacity
        /// <para>
        /// The capacity units consumed by the entire <c>BatchWriteItem</c> operation.
        /// </para>
        ///  
        /// <para>
        /// Each element consists of:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>TableName</c> - The table that consumed the provisioned throughput.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>CapacityUnits</c> - The total number of capacity units consumed.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public List<ConsumedCapacity>? ConsumedCapacity { get; set; }
    }
}
