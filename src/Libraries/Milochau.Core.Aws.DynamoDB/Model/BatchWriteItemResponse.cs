using Milochau.Core.Aws.DynamoDB.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of a <c>BatchWriteItem</c> operation.
    /// </summary>
    public class BatchWriteItemResponse : AmazonDynamoDBResponse
    {
        /// <summary>
        /// ItemCollectionMetrics
        /// <para>
        /// A list of tables that were processed by <c>BatchWriteItem</c> and, for each
        /// table, information about any item collections that were affected by individual <c>DeleteItem</c>
        /// or <c>PutItem</c> operations.
        /// </para>
        ///  
        /// <para>
        /// Each entry consists of the following subelements:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>ItemCollectionKey</c> - The partition key value of the item collection.
        /// This is the same as the partition key value of the item.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>SizeEstimateRangeGB</c> - An estimate of item collection size, expressed
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
        /// UnprocessedItems
        /// <para>
        /// A map of tables and requests against those tables that were not processed. The <c>UnprocessedItems</c>
        /// value is in the same form as <c>RequestItems</c>, so you can provide this value
        /// directly to a subsequent <c>BatchWriteItem</c> operation. For more information,
        /// see <c>RequestItems</c> in the Request Parameters section.
        /// </para>
        ///  
        /// <para>
        /// Each <c>UnprocessedItems</c> entry consists of a table name and, for that table,
        /// a list of operations to perform (<c>DeleteRequest</c> or <c>PutRequest</c>).
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>DeleteRequest</c> - Perform a <c>DeleteItem</c> operation on the specified
        /// item. The item to be deleted is identified by a <c>Key</c> subelement:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>Key</c> - A map of primary key attribute values that uniquely identify the
        /// item. Each entry in this map consists of an attribute name and an attribute value.
        /// </para>
        ///  </li> </ul> </li> <li> 
        /// <para>
        ///  <c>PutRequest</c> - Perform a <c>PutItem</c> operation on the specified
        /// item. The item to be put is identified by an <c>Item</c> subelement:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>Item</c> - A map of attributes and their values. Each entry in this map
        /// consists of an attribute name and an attribute value. Attribute values must not be
        /// null; string and binary type attributes must have lengths greater than zero; and set
        /// type attributes must not be empty. Requests that contain empty values will be rejected
        /// with a <c>ValidationException</c> exception.
        /// </para>
        ///  
        /// <para>
        /// If you specify any attributes that are part of an index key, then the data types for
        /// those attributes must match those of the schema in the table's attribute definition.
        /// </para>
        ///  </li> </ul> </li> </ul> 
        /// <para>
        /// If there are no unprocessed items remaining, the response contains an empty <c>UnprocessedItems</c>
        /// map.
        /// </para>
        /// </summary>
        public Dictionary<string, List<WriteRequest>>? UnprocessedItems { get; set; }
    }

    /// <inheritdoc/>
    public class BatchWriteItemResponse<TEntity> : BatchWriteItemResponse
        where TEntity : class, IDynamoDbBatchWritableEntity<TEntity>
    {
        /// <inheritdoc/>
        public BatchWriteItemResponse(BatchWriteItemResponse response)
        {
            ConsumedCapacity = response.ConsumedCapacity;
            HttpStatusCode = response.HttpStatusCode;
            ResponseMetadata = response.ResponseMetadata;

            ItemCollectionMetrics = response.ItemCollectionMetrics;
            UnprocessedItems = response.UnprocessedItems;

            if (response.UnprocessedItems != null)
            {
                UnprocessedPutRequests = response.UnprocessedItems[TEntity.TableName]
                    .Select(x => x.PutRequest?.Item != null ? TEntity.ParseFromDynamoDb(x.PutRequest.Item) : null)
                    .Where(x => x != null)
                    .Select(x => new PutRequest<TEntity>
                    {
                        Entity = x!,
                    })
                    .ToList();

                UnprocessedDeleteRequests = response.UnprocessedItems[TEntity.TableName]
                    .Select(x => x.DeleteRequest?.Key)
                    .Where(x => x != null)
                    .Select(x => new DeleteRequest<TEntity>
                    {
                        PartitionKey = x![TEntity.PartitionKey],
                        SortKey = x!.GetValueOrDefault(TEntity.SortKey),
                    })
                    .ToList();
            }
        }

        /// <summary>Unprocessed Put requests</summary>
        public List<PutRequest<TEntity>>? UnprocessedPutRequests { get; set; }

        /// <summary>Unprocessed Delete requests</summary>
        public List<DeleteRequest<TEntity>>? UnprocessedDeleteRequests { get; set; }
    }
}