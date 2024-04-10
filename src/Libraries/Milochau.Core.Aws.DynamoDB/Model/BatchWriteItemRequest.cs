using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.DynamoDB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Container for the parameters to the BatchWriteItem operation.
    /// The <c>BatchWriteItem</c> operation puts or deletes multiple items in one or
    /// more tables. A single call to <c>BatchWriteItem</c> can transmit up to 16MB
    /// of data over the network, consisting of up to 25 item put or delete operations. While
    /// individual items can be up to 400 KB once stored, it's important to note that an item's
    /// representation might be greater than 400KB while being sent in DynamoDB's JSON format
    /// for the API call. For more details on this distinction, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.NamingRulesDataTypes.html">Naming
    /// Rules and Data Types</a>.
    /// 
    /// <para>
    ///  <c>BatchWriteItem</c> cannot update items. If you perform a <c>BatchWriteItem</c>
    /// operation on an existing item, that item's values will be overwritten by the operation
    /// and it will appear like it was updated. To update items, we recommend you use the
    /// <c>UpdateItem</c> action.
    /// </para>
    /// <para>
    /// The individual <c>PutItem</c> and <c>DeleteItem</c> operations specified
    /// in <c>BatchWriteItem</c> are atomic; however <c>BatchWriteItem</c> as
    /// a whole is not. If any requested operations fail because the table's provisioned throughput
    /// is exceeded or an internal processing failure occurs, the failed operations are returned
    /// in the <c>UnprocessedItems</c> response parameter. You can investigate and optionally
    /// resend the requests. Typically, you would call <c>BatchWriteItem</c> in a loop.
    /// Each iteration would check for unprocessed items and submit a new <c>BatchWriteItem</c>
    /// request with those unprocessed items until all items have been processed.
    /// </para>
    ///  
    /// <para>
    /// If <i>none</i> of the items can be processed due to insufficient provisioned throughput
    /// on all of the tables in the request, then <c>BatchWriteItem</c> returns a <c>ProvisionedThroughputExceededException</c>.
    /// </para>
    ///  <important> 
    /// <para>
    /// If DynamoDB returns any unprocessed items, you should retry the batch operation on
    /// those items. However, <i>we strongly recommend that you use an exponential backoff
    /// algorithm</i>. If you retry the batch operation immediately, the underlying read or
    /// write requests can still fail due to throttling on the individual tables. If you delay
    /// the batch operation using exponential backoff, the individual requests in the batch
    /// are much more likely to succeed.
    /// </para>
    ///  
    /// <para>
    /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ErrorHandling.html#Programming.Errors.BatchOperations">Batch
    /// Operations and Error Handling</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </para>
    ///  </important> 
    /// <para>
    /// With <c>BatchWriteItem</c>, you can efficiently write or delete large amounts
    /// of data, such as from Amazon EMR, or copy data from another database into DynamoDB.
    /// In order to improve performance with these large-scale operations, <c>BatchWriteItem</c>
    /// does not behave in the same way as individual <c>PutItem</c> and <c>DeleteItem</c>
    /// calls would. For example, you cannot specify conditions on individual put and delete
    /// requests, and <c>BatchWriteItem</c> does not return deleted items in the response.
    /// </para>
    ///  
    /// <para>
    /// If you use a programming language that supports concurrency, you can use threads to
    /// write items in parallel. Your application must include the necessary logic to manage
    /// the threads. With languages that don't support threading, you must update or delete
    /// the specified items one at a time. In both situations, <c>BatchWriteItem</c>
    /// performs the specified put and delete operations in parallel, giving you the power
    /// of the thread pool approach without having to introduce complexity into your application.
    /// </para>
    ///  
    /// <para>
    /// Parallel processing reduces latency, but each specified put and delete request consumes
    /// the same number of write capacity units whether it is processed in parallel or not.
    /// Delete operations on nonexistent items consume one write capacity unit.
    /// </para>
    ///  
    /// <para>
    /// If one or more of the following is true, DynamoDB rejects the entire batch write operation:
    /// </para>
    ///  <ul> <li> 
    /// <para>
    /// One or more tables specified in the <c>BatchWriteItem</c> request does not exist.
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// Primary key attributes specified on an item in the request do not match those in the
    /// corresponding table's primary key schema.
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// You try to perform multiple operations on the same item in the same <c>BatchWriteItem</c>
    /// request. For example, you cannot put and delete the same item in the same <c>BatchWriteItem</c>
    /// request. 
    /// </para>
    ///  </li> <li> 
    /// <para>
    ///  Your request contains at least two items with identical hash and range keys (which
    /// essentially is two put operations). 
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// There are more than 25 requests in the batch.
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// Any individual item in a batch exceeds 400 KB.
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// The total request size exceeds 16 MB.
    /// </para>
    ///  </li> </ul>
    /// </summary>
    public class BatchWriteItemRequest(Guid? userId) : AmazonDynamoDBRequest(userId)
    {
        /// <summary>
        /// RequestItems
        /// <para>
        /// A map of one or more table names and, for each table, a list of operations to be performed
        /// (<c>DeleteRequest</c> or <c>PutRequest</c>). Each element in the map consists
        /// of the following:
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
        /// For each primary key, you must provide <i>all</i> of the key attributes. For example,
        /// with a simple primary key, you only need to provide a value for the partition key.
        /// For a composite primary key, you must provide values for <i>both</i> the partition
        /// key and the sort key.
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
        /// type attributes must not be empty. Requests that contain empty values are rejected
        /// with a <c>ValidationException</c> exception.
        /// </para>
        ///  
        /// <para>
        /// If you specify any attributes that are part of an index key, then the data types for
        /// those attributes must match those of the schema in the table's attribute definition.
        /// </para>
        ///  </li> </ul> </li> </ul>
        /// </summary>
        public Dictionary<string, List<WriteRequest>>? RequestItems { get; set; }

        /// <summary>
        /// ReturnItemCollectionMetrics
        /// <para>
        /// Determines whether item collection metrics are returned. If set to <c>SIZE</c>,
        /// the response includes statistics about item collections, if any, that were modified
        /// during the operation are returned in the response. If set to <c>NONE</c> (the
        /// default), no statistics are returned.
        /// </para>
        /// </summary>
        public ReturnItemCollectionMetrics? ReturnItemCollectionMetrics { get; set; }
    }

    /// <inheritdoc cref="BatchWriteItemRequest"/>
    public class BatchWriteItemRequest<TEntity>
        where TEntity : class, IDynamoDbBatchWritableEntity<TEntity>
    {
        /// <inheritdoc cref="AmazonWebServiceRequest.UserId"/>
        public required Guid? UserId { get; set; }

        /// <inheritdoc cref="AmazonDynamoDBRequest.ReturnConsumedCapacity"/>
        public ReturnConsumedCapacity? ReturnConsumedCapacity { get; set; }


        /// <summary>Request entities</summary>
        public required List<WriteRequest<TEntity>> RequestEntities { get; set; }


        /// <inheritdoc cref="BatchWriteItemRequest.ReturnItemCollectionMetrics"/>
        public ReturnItemCollectionMetrics? ReturnItemCollectionMetrics { get; set; }

        internal IEnumerable<BatchWriteItemRequest> Build()
        {
            var requests = RequestEntities.Select(x => x.Build()).ToList();
            var maxBatchItems = 25;

            for (var i = 0; i < requests.Count; i += maxBatchItems)
            {
                var items = requests.Skip(i).Take(maxBatchItems).ToList();
                var requestItems = new Dictionary<string, List<WriteRequest>>
                {
                    [TEntity.TableName] = items,
                };

                yield return new BatchWriteItemRequest(UserId)
                {
                    ReturnConsumedCapacity = ReturnConsumedCapacity,
                    ReturnItemCollectionMetrics = ReturnItemCollectionMetrics,
                    RequestItems = requestItems,
                };
            }
        }
    }
}