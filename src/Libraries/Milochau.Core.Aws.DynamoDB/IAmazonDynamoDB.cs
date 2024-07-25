using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>
    /// Interface for accessing DynamoDB
    /// <para>
    /// Amazon DynamoDB is a fully managed NoSQL database service that provides fast and predictable
    /// performance with seamless scalability. DynamoDB lets you offload the administrative
    /// burdens of operating and scaling a distributed database, so that you don't have to
    /// worry about hardware provisioning, setup and configuration, replication, software
    /// patching, or cluster scaling.
    /// </para>
    /// <para>
    /// With DynamoDB, you can create database tables that can store and retrieve any amount
    /// of data, and serve any level of request traffic. You can scale up or scale down your
    /// tables' throughput capacity without downtime or performance degradation, and use the
    /// Amazon Web Services Management Console to monitor resource utilization and performance
    /// metrics.
    /// </para>
    /// <para>
    /// DynamoDB automatically spreads the data and traffic for your tables over a sufficient
    /// number of servers to handle your throughput and storage requirements, while maintaining
    /// consistent and fast performance. All of your data is stored on solid state disks (SSDs)
    /// and automatically replicated across multiple Availability Zones in an Amazon Web Services
    /// Region, providing built-in high availability and data durability.
    /// </para>
    /// </summary>
    public partial interface IAmazonDynamoDB
    {
        /// <summary>
        /// The <c>BatchWriteItem</c> operation puts or deletes multiple items in one or
        /// more tables. A single call to <c>BatchWriteItem</c> can transmit up to 16MB
        /// of data over the network, consisting of up to 25 item put or delete operations. While
        /// individual items can be up to 400 KB once stored, it's important to note that an item's
        /// representation might be greater than 400KB while being sent in DynamoDB's JSON format
        /// for the API call. For more details on this distinction, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.NamingRulesDataTypes.html">Naming
        /// Rules and Data Types</a>.
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
        /// <para>
        /// If you use a programming language that supports concurrency, you can use threads to
        /// write items in parallel. Your application must include the necessary logic to manage
        /// the threads. With languages that don't support threading, you must update or delete
        /// the specified items one at a time. In both situations, <c>BatchWriteItem</c>
        /// performs the specified put and delete operations in parallel, giving you the power
        /// of the thread pool approach without having to introduce complexity into your application.
        /// </para>
        /// <para>
        /// Parallel processing reduces latency, but each specified put and delete request consumes
        /// the same number of write capacity units whether it is processed in parallel or not.
        /// Delete operations on nonexistent items consume one write capacity unit.
        /// </para>
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
        /// <returns>The response from the BatchWriteItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/BatchWriteItem">REST API Reference for BatchWriteItem Operation</seealso>
        Task<List<BatchWriteItemResponse<TEntity>>> BatchWriteItemAsync<TEntity>(BatchWriteItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity: class, IDynamoDbBatchWritableEntity<TEntity>;

        /// <summary>Unmanaged <c>BatchWriteItemAsync</c> operation.</summary>
        Task<BatchWriteItemResponse> BatchWriteItemAsync(BatchWriteItemRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// Deletes a single item in a table by primary key. You can perform a conditional delete
        /// operation that deletes the item if it exists, or if it has an expected attribute value.
        /// <para>
        /// In addition to deleting an item, you can also return the item's attribute values in
        /// the same operation, using the <c>ReturnValues</c> parameter.
        /// </para>
        /// <para>
        /// Unless you specify conditions, the <c>DeleteItem</c> is an idempotent operation;
        /// running it multiple times on the same item or attribute does <i>not</i> result in
        /// an error response.
        /// </para>
        /// <para>
        /// Conditional deletes are useful for deleting items only if specific conditions are
        /// met. If those conditions are met, DynamoDB performs the delete. Otherwise, the item
        /// is not deleted.
        /// </para>
        /// </summary>
        /// <returns>The response from the DeleteItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/DeleteItem">REST API Reference for DeleteItem Operation</seealso>
        Task<DeleteItemResponse<TEntity>> DeleteItemAsync<TEntity>(DeleteItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbDeletableEntity<TEntity>;

        /// <summary>Unmanaged <c>DeleteItemAsync</c> operation.</summary>
        Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// The <c>GetItem</c> operation returns a set of attributes for the item with the
        /// given primary key. If there is no matching item, <c>GetItem</c> does not return
        /// any data and there will be no <c>Item</c> element in the response.
        /// <para>
        ///  <c>GetItem</c> provides an eventually consistent read by default. If your application
        /// requires a strongly consistent read, set <c>ConsistentRead</c> to <c>true</c>.
        /// Although a strongly consistent read might take more time than an eventually consistent
        /// read, it always returns the last updated value.
        /// </para>
        /// </summary>
        /// <returns>The response from the GetItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/GetItem">REST API Reference for GetItem Operation</seealso>
        Task<GetItemResponse<TEntity>> GetItemAsync<TEntity>(GetItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbGettableEntity<TEntity>;

        /// <summary>Unmanaged <c>GetItemAsync</c> operation.</summary>
        Task<GetItemResponse> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// Creates a new item, or replaces an old item with a new item. If an item that has the
        /// same primary key as the new item already exists in the specified table, the new item
        /// completely replaces the existing item. You can perform a conditional put operation
        /// (add a new item if one with the specified primary key doesn't exist), or replace an
        /// existing item if it has certain attribute values. You can return the item's attribute
        /// values in the same operation, using the <c>ReturnValues</c> parameter.
        /// <para>
        /// When you add an item, the primary key attributes are the only required attributes.
        /// </para>
        /// <para>
        /// Empty String and Binary attribute values are allowed. Attribute values of type String
        /// and Binary must have a length greater than zero if the attribute is used as a key
        /// attribute for a table or index. Set type attributes cannot be empty. 
        /// </para>
        /// <para>
        /// Invalid Requests with empty values will be rejected with a <c>ValidationException</c>
        /// exception.
        /// </para>
        /// <para>
        /// To prevent a new item from replacing an existing item, use a conditional expression
        /// that contains the <c>attribute_not_exists</c> function with the name of the
        /// attribute being used as the partition key for the table. Since every record must contain
        /// that attribute, the <c>attribute_not_exists</c> function will only succeed if
        /// no matching item exists.
        /// </para>
        /// <para>
        /// For more information about <c>PutItem</c>, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/WorkingWithItems.html">Working
        /// with Items</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        /// <returns>The response from the PutItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/PutItem">REST API Reference for PutItem Operation</seealso>
        Task<PutItemResponse<TEntity>> PutItemAsync<TEntity>(PutItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbPutableEntity<TEntity>;

        /// <summary>Unmanaged <c>PutItemAsync</c> operation.</summary>
        Task<PutItemResponse> PutItemAsync(PutItemRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// Edits an existing item's attributes, or adds a new item to the table if it does not
        /// already exist. You can put, delete, or add attribute values. You can also perform
        /// a conditional update on an existing item (insert a new attribute name-value pair if
        /// it doesn't exist, or replace an existing name-value pair if it has certain expected
        /// attribute values).
        /// <para>
        /// You can also return the item's attribute values in the same <c>UpdateItem</c>
        /// operation using the <c>ReturnValues</c> parameter.
        /// </para>
        /// </summary>
        /// <returns>The response from the UpdateItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/UpdateItem">REST API Reference for UpdateItem Operation</seealso>
        Task<UpdateItemResponse<TEntity>> UpdateItemAsync<TEntity>(UpdateItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbUpdatableEntity<TEntity>;

        /// <summary>Unmanaged <c>UpdateItemAsync</c> operation.</summary>
        Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// You must provide the name of the partition key attribute and a single value for that
        /// attribute. <c>Query</c> returns all items with that partition key value. Optionally,
        /// you can provide a sort key attribute and use a comparison operator to refine the search
        /// results.
        /// <para>
        /// Use the <c>KeyConditionExpression</c> parameter to provide a specific value
        /// for the partition key. The <c>Query</c> operation will return all of the items
        /// from the table or index with that partition key value. You can optionally narrow the
        /// scope of the <c>Query</c> operation by specifying a sort key value and a comparison
        /// operator in <c>KeyConditionExpression</c>. To further refine the <c>Query</c>
        /// results, you can optionally provide a <c>FilterExpression</c>. A <c>FilterExpression</c>
        /// determines which items within the results should be returned to you. All of the other
        /// results are discarded. 
        /// </para>
        /// <para>
        ///  A <c>Query</c> operation always returns a result set. If no matching items
        /// are found, the result set will be empty. Queries that do not return results consume
        /// the minimum number of read capacity units for that type of read operation. 
        /// </para>
        /// <para>
        ///  DynamoDB calculates the number of read capacity units consumed based on item size,
        /// not on the amount of data that is returned to an application. The number of capacity
        /// units consumed will be the same whether you request all of the attributes (the default
        /// behavior) or just some of them (using a projection expression). The number will also
        /// be the same whether or not you use a <c>FilterExpression</c>. 
        /// </para>
        /// <para>
        ///  <c>Query</c> results are always sorted by the sort key value. If the data type
        /// of the sort key is Number, the results are returned in numeric order; otherwise, the
        /// results are returned in order of UTF-8 bytes. By default, the sort order is ascending.
        /// To reverse the order, set the <c>ScanIndexForward</c> parameter to false. 
        /// </para>
        /// <para>
        ///  A single <c>Query</c> operation will read up to the maximum number of items
        /// set (if using the <c>Limit</c> parameter) or a maximum of 1 MB of data and then
        /// apply any filtering to the results using <c>FilterExpression</c>. If <c>LastEvaluatedKey</c>
        /// is present in the response, you will need to paginate the result set. For more information,
        /// see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Query.html#Query.Pagination">Paginating
        /// the Results</a> in the <i>Amazon DynamoDB Developer Guide</i>. 
        /// </para>
        /// <para>
        ///  <c>FilterExpression</c> is applied after a <c>Query</c> finishes, but
        /// before the results are returned. A <c>FilterExpression</c> cannot contain partition
        /// key or sort key attributes. You need to specify those attributes in the <c>KeyConditionExpression</c>.
        /// </para>
        /// <para>
        ///  A <c>Query</c> operation can return an empty result set and a <c>LastEvaluatedKey</c>
        /// if all the items read for the page of results are filtered out. 
        /// </para>
        /// <para>
        /// You can query a table, a local secondary index, or a global secondary index. For a
        /// query on a table or on a local secondary index, you can set the <c>ConsistentRead</c>
        /// parameter to <c>true</c> and obtain a strongly consistent result. Global secondary
        /// indexes support eventually consistent reads only, so do not specify <c>ConsistentRead</c>
        /// when querying a global secondary index.
        /// </para>
        /// </summary>
        /// <returns>The response from the Query service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/Query">REST API Reference for Query Operation</seealso>
        Task<QueryResponse<TEntity>> QueryAsync<TEntity>(QueryRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbQueryableEntity<TEntity>;

        /// <summary>Unmanaged <c>QueryAsync</c> operation.</summary>
        Task<QueryResponse> QueryAsync(QueryRequest request, CancellationToken cancellationToken);


        /// <summary>
        /// The <c>Scan</c> operation returns one or more items and item attributes by accessing
        /// every item in a table or a secondary index. To have DynamoDB return fewer items, you
        /// can provide a <c>FilterExpression</c> operation.
        /// <para>
        /// If the total size of scanned items exceeds the maximum dataset size limit of 1 MB,
        /// the scan completes and results are returned to the user. The <c>LastEvaluatedKey</c>
        /// value is also returned and the requestor can use the <c>LastEvaluatedKey</c> to continue
        /// the scan in a subsequent operation. Each scan response also includes number of items
        /// that were scanned (ScannedCount) as part of the request. If using a <c>FilterExpression</c>,
        /// a scan result can result in no items meeting the criteria and the <c>Count</c> will
        /// result in zero. If you did not use a <c>FilterExpression</c> in the scan request,
        /// then <c>Count</c> is the same as <c>ScannedCount</c>.
        /// </para>
        /// <para>
        ///  <c>Count</c> and <c>ScannedCount</c> only return the count of items specific to a
        /// single scan request and, unless the table is less than 1MB, do not represent the total
        /// number of items in the table. 
        /// </para>
        /// <para>
        /// A single <c>Scan</c> operation first reads up to the maximum number of items set (if
        /// using the <c>Limit</c> parameter) or a maximum of 1 MB of data and then applies any
        /// filtering to the results if a <c>FilterExpression</c> is provided. If <c>LastEvaluatedKey</c>
        /// is present in the response, pagination is required to complete the full table scan.
        /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Scan.html#Scan.Pagination">Paginating
        /// the Results</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// <para>
        ///  <c>Scan</c> operations proceed sequentially; however, for faster performance on a
        /// large table or secondary index, applications can request a parallel <c>Scan</c> operation
        /// by providing the <c>Segment</c> and <c>TotalSegments</c> parameters. For more information,
        /// see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Scan.html#Scan.ParallelScan">Parallel
        /// Scan</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// <para>
        /// By default, a <c>Scan</c> uses eventually consistent reads when accessing the items
        /// in a table. Therefore, the results from an eventually consistent <c>Scan</c> may not
        /// include the latest item changes at the time the scan iterates through each item in
        /// the table. If you require a strongly consistent read of each item as the scan iterates
        /// through the items in the table, you can set the <c>ConsistentRead</c> parameter to
        /// true. Strong consistency only relates to the consistency of the read at the item level.
        /// </para>
        /// <para>
        ///  DynamoDB does not provide snapshot isolation for a scan operation when the <c>ConsistentRead</c>
        /// parameter is set to true. Thus, a DynamoDB scan operation does not guarantee that
        /// all reads in a scan see a consistent snapshot of the table when the scan operation
        /// was requested. 
        /// </para>
        /// </summary>
        /// <returns>The response from the Scan service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/Scan">REST API Reference for Scan Operation</seealso>
        Task<ScanResponse<TEntity>> ScanAsync<TEntity>(ScanRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbScanableEntity<TEntity>;

        /// <summary>Unmanaged <c>ScanAsync</c> operation.</summary>
        Task<ScanResponse> ScanAsync(ScanRequest request, CancellationToken cancellationToken);
    }
}
