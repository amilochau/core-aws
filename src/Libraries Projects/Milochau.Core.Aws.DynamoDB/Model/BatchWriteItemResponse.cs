using Amazon.Runtime;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of a <code>BatchWriteItem</code> operation.
    /// </summary>
    public partial class BatchWriteItemResponse : AmazonWebServiceResponse
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

        /// <summary>
        /// Gets and sets the property ItemCollectionMetrics. 
        /// <para>
        /// A list of tables that were processed by <code>BatchWriteItem</code> and, for each
        /// table, information about any item collections that were affected by individual <code>DeleteItem</code>
        /// or <code>PutItem</code> operations.
        /// </para>
        ///  
        /// <para>
        /// Each entry consists of the following subelements:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>ItemCollectionKey</code> - The partition key value of the item collection.
        /// This is the same as the partition key value of the item.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>SizeEstimateRangeGB</code> - An estimate of item collection size, expressed
        /// in GB. This is a two-element array containing a lower bound and an upper bound for
        /// the estimate. The estimate includes the size of all the items in the table, plus the
        /// size of all attributes projected into all of the local secondary indexes on the table.
        /// Use this estimate to measure whether a local secondary index is approaching its size
        /// limit.
        /// </para>
        ///  
        /// <para>
        /// The estimate is subject to change over time; therefore, do not rely on the precision
        /// or accuracy of the estimate.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public Dictionary<string, List<ItemCollectionMetrics>>? ItemCollectionMetrics { get; set; }

        /// <summary>
        /// Gets and sets the property UnprocessedItems. 
        /// <para>
        /// A map of tables and requests against those tables that were not processed. The <code>UnprocessedItems</code>
        /// value is in the same form as <code>RequestItems</code>, so you can provide this value
        /// directly to a subsequent <code>BatchWriteItem</code> operation. For more information,
        /// see <code>RequestItems</code> in the Request Parameters section.
        /// </para>
        ///  
        /// <para>
        /// Each <code>UnprocessedItems</code> entry consists of a table name and, for that table,
        /// a list of operations to perform (<code>DeleteRequest</code> or <code>PutRequest</code>).
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>DeleteRequest</code> - Perform a <code>DeleteItem</code> operation on the specified
        /// item. The item to be deleted is identified by a <code>Key</code> subelement:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>Key</code> - A map of primary key attribute values that uniquely identify the
        /// item. Each entry in this map consists of an attribute name and an attribute value.
        /// </para>
        ///  </li> </ul> </li> <li> 
        /// <para>
        ///  <code>PutRequest</code> - Perform a <code>PutItem</code> operation on the specified
        /// item. The item to be put is identified by an <code>Item</code> subelement:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>Item</code> - A map of attributes and their values. Each entry in this map
        /// consists of an attribute name and an attribute value. Attribute values must not be
        /// null; string and binary type attributes must have lengths greater than zero; and set
        /// type attributes must not be empty. Requests that contain empty values will be rejected
        /// with a <code>ValidationException</code> exception.
        /// </para>
        ///  
        /// <para>
        /// If you specify any attributes that are part of an index key, then the data types for
        /// those attributes must match those of the schema in the table's attribute definition.
        /// </para>
        ///  </li> </ul> </li> </ul> 
        /// <para>
        /// If there are no unprocessed items remaining, the response contains an empty <code>UnprocessedItems</code>
        /// map.
        /// </para>
        /// </summary>
        public Dictionary<string, List<WriteRequest>>? UnprocessedItems { get; set; }
    }
}