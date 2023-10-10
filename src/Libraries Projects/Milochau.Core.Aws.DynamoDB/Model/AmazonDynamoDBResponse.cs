using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Base class for DynamoDB operation responses.
    /// </summary>
    public partial class AmazonDynamoDBResponse : AmazonWebServiceResponse
    {
        /// <summary>
        /// Gets and sets the property ConsumedCapacity. 
        /// <para>
        /// The capacity units consumed by the entire <code>BatchWriteItem</code> operation.
        /// </para>
        ///  
        /// <para>
        /// Each element consists of:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>TableName</code> - The table that consumed the provisioned throughput.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>CapacityUnits</code> - The total number of capacity units consumed.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public List<ConsumedCapacity>? ConsumedCapacity { get; set; }
    }
}
