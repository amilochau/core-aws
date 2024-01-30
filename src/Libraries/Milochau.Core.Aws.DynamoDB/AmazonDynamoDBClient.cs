using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.DynamoDB.Model;
using Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>
    /// Implementation for accessing DynamoDB
    /// </summary>
    public partial class AmazonDynamoDBClient : AmazonServiceClient, IAmazonDynamoDB
    {
        #region Constructors

        /// <summary>
        /// Constructs AmazonDynamoDBClient with the credentials loaded from the application's
        /// default configuration, and if unsuccessful from the Instance Profile service on an EC2 instance.
        /// </summary>
        public AmazonDynamoDBClient()
            : base(new ClientConfig
            {
                AuthenticationServiceName = "dynamodb",
                MonitoringServiceName = "DynamoDBv2"
            })
        { }

        #endregion

        #region  BatchWriteItem

        /// <summary>
        /// The <code>BatchWriteItem</code> operation puts or deletes multiple items in one or
        /// more tables. A single call to <code>BatchWriteItem</code> can transmit up to 16MB
        /// of data over the network, consisting of up to 25 item put or delete operations. While
        /// individual items can be up to 400 KB once stored, it's important to note that an item's
        /// representation might be greater than 400KB while being sent in DynamoDB's JSON format
        /// for the API call. For more details on this distinction, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.NamingRulesDataTypes.html">Naming
        /// Rules and Data Types</a>.
        /// 
        ///  <note> 
        /// <para>
        ///  <code>BatchWriteItem</code> cannot update items. If you perform a <code>BatchWriteItem</code>
        /// operation on an existing item, that item's values will be overwritten by the operation
        /// and it will appear like it was updated. To update items, we recommend you use the
        /// <code>UpdateItem</code> action.
        /// </para>
        ///  </note> 
        /// <para>
        /// The individual <code>PutItem</code> and <code>DeleteItem</code> operations specified
        /// in <code>BatchWriteItem</code> are atomic; however <code>BatchWriteItem</code> as
        /// a whole is not. If any requested operations fail because the table's provisioned throughput
        /// is exceeded or an internal processing failure occurs, the failed operations are returned
        /// in the <code>UnprocessedItems</code> response parameter. You can investigate and optionally
        /// resend the requests. Typically, you would call <code>BatchWriteItem</code> in a loop.
        /// Each iteration would check for unprocessed items and submit a new <code>BatchWriteItem</code>
        /// request with those unprocessed items until all items have been processed.
        /// </para>
        ///  
        /// <para>
        /// If <i>none</i> of the items can be processed due to insufficient provisioned throughput
        /// on all of the tables in the request, then <code>BatchWriteItem</code> returns a <code>ProvisionedThroughputExceededException</code>.
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
        /// With <code>BatchWriteItem</code>, you can efficiently write or delete large amounts
        /// of data, such as from Amazon EMR, or copy data from another database into DynamoDB.
        /// In order to improve performance with these large-scale operations, <code>BatchWriteItem</code>
        /// does not behave in the same way as individual <code>PutItem</code> and <code>DeleteItem</code>
        /// calls would. For example, you cannot specify conditions on individual put and delete
        /// requests, and <code>BatchWriteItem</code> does not return deleted items in the response.
        /// </para>
        ///  
        /// <para>
        /// If you use a programming language that supports concurrency, you can use threads to
        /// write items in parallel. Your application must include the necessary logic to manage
        /// the threads. With languages that don't support threading, you must update or delete
        /// the specified items one at a time. In both situations, <code>BatchWriteItem</code>
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
        /// One or more tables specified in the <code>BatchWriteItem</code> request does not exist.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Primary key attributes specified on an item in the request do not match those in the
        /// corresponding table's primary key schema.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// You try to perform multiple operations on the same item in the same <code>BatchWriteItem</code>
        /// request. For example, you cannot put and delete the same item in the same <code>BatchWriteItem</code>
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
        /// <param name="request">Container for the necessary parameters to execute the BatchWriteItem service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the BatchWriteItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/BatchWriteItem">REST API Reference for BatchWriteItem Operation</seealso>
        public virtual Task<BatchWriteItemResponse> BatchWriteItemAsync(BatchWriteItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = BatchWriteItemRequestMarshaller.Instance,
                ResponseUnmarshaller = BatchWriteItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "BatchWriteItem",
            };

            return InvokeAsync<BatchWriteItemResponse>(request, options, cancellationToken);
        }

        #endregion
        #region  DeleteItem

        /// <summary>
        /// Deletes a single item in a table by primary key. You can perform a conditional delete
        /// operation that deletes the item if it exists, or if it has an expected attribute value.
        /// 
        ///  
        /// <para>
        /// In addition to deleting an item, you can also return the item's attribute values in
        /// the same operation, using the <code>ReturnValues</code> parameter.
        /// </para>
        ///  
        /// <para>
        /// Unless you specify conditions, the <code>DeleteItem</code> is an idempotent operation;
        /// running it multiple times on the same item or attribute does <i>not</i> result in
        /// an error response.
        /// </para>
        ///  
        /// <para>
        /// Conditional deletes are useful for deleting items only if specific conditions are
        /// met. If those conditions are met, DynamoDB performs the delete. Otherwise, the item
        /// is not deleted.
        /// </para>
        /// </summary>
        /// <param name="request">Container for the necessary parameters to execute the DeleteItem service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the DeleteItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/DeleteItem">REST API Reference for DeleteItem Operation</seealso>
        public virtual Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = DeleteItemRequestMarshaller.Instance,
                ResponseUnmarshaller = DeleteItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "DeleteItem",
            };

            return InvokeAsync<DeleteItemResponse>(request, options, cancellationToken);
        }

        #endregion
        #region  GetItem

        /// <summary>
        /// The <code>GetItem</code> operation returns a set of attributes for the item with the
        /// given primary key. If there is no matching item, <code>GetItem</code> does not return
        /// any data and there will be no <code>Item</code> element in the response.
        /// 
        ///  
        /// <para>
        ///  <code>GetItem</code> provides an eventually consistent read by default. If your application
        /// requires a strongly consistent read, set <code>ConsistentRead</code> to <code>true</code>.
        /// Although a strongly consistent read might take more time than an eventually consistent
        /// read, it always returns the last updated value.
        /// </para>
        /// </summary>
        /// <param name="request">Container for the necessary parameters to execute the GetItem service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the GetItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/GetItem">REST API Reference for GetItem Operation</seealso>
        public virtual Task<GetItemResponse> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = GetItemRequestMarshaller.Instance,
                ResponseUnmarshaller = GetItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "GetItem",
            };

            return InvokeAsync<GetItemResponse>(request, options, cancellationToken);
        }

        #endregion
        #region  PutItem

        /// <summary>
        /// Creates a new item, or replaces an old item with a new item. If an item that has the
        /// same primary key as the new item already exists in the specified table, the new item
        /// completely replaces the existing item. You can perform a conditional put operation
        /// (add a new item if one with the specified primary key doesn't exist), or replace an
        /// existing item if it has certain attribute values. You can return the item's attribute
        /// values in the same operation, using the <code>ReturnValues</code> parameter.
        /// 
        ///  
        /// <para>
        /// When you add an item, the primary key attributes are the only required attributes.
        /// 
        /// </para>
        ///  
        /// <para>
        /// Empty String and Binary attribute values are allowed. Attribute values of type String
        /// and Binary must have a length greater than zero if the attribute is used as a key
        /// attribute for a table or index. Set type attributes cannot be empty. 
        /// </para>
        ///  
        /// <para>
        /// Invalid Requests with empty values will be rejected with a <code>ValidationException</code>
        /// exception.
        /// </para>
        ///  <note> 
        /// <para>
        /// To prevent a new item from replacing an existing item, use a conditional expression
        /// that contains the <code>attribute_not_exists</code> function with the name of the
        /// attribute being used as the partition key for the table. Since every record must contain
        /// that attribute, the <code>attribute_not_exists</code> function will only succeed if
        /// no matching item exists.
        /// </para>
        ///  </note> 
        /// <para>
        /// For more information about <code>PutItem</code>, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/WorkingWithItems.html">Working
        /// with Items</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        /// <param name="request">Container for the necessary parameters to execute the PutItem service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the PutItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/PutItem">REST API Reference for PutItem Operation</seealso>
        public virtual Task<PutItemResponse> PutItemAsync(PutItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = PutItemRequestMarshaller.Instance,
                ResponseUnmarshaller = PutItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "PutItem",
            };

            return InvokeAsync<PutItemResponse>(request, options, cancellationToken);
        }

        #endregion
        #region  Query

        /// <summary>
        /// You must provide the name of the partition key attribute and a single value for that
        /// attribute. <code>Query</code> returns all items with that partition key value. Optionally,
        /// you can provide a sort key attribute and use a comparison operator to refine the search
        /// results.
        /// 
        ///  
        /// <para>
        /// Use the <code>KeyConditionExpression</code> parameter to provide a specific value
        /// for the partition key. The <code>Query</code> operation will return all of the items
        /// from the table or index with that partition key value. You can optionally narrow the
        /// scope of the <code>Query</code> operation by specifying a sort key value and a comparison
        /// operator in <code>KeyConditionExpression</code>. To further refine the <code>Query</code>
        /// results, you can optionally provide a <code>FilterExpression</code>. A <code>FilterExpression</code>
        /// determines which items within the results should be returned to you. All of the other
        /// results are discarded. 
        /// </para>
        ///  
        /// <para>
        ///  A <code>Query</code> operation always returns a result set. If no matching items
        /// are found, the result set will be empty. Queries that do not return results consume
        /// the minimum number of read capacity units for that type of read operation. 
        /// </para>
        ///  <note> 
        /// <para>
        ///  DynamoDB calculates the number of read capacity units consumed based on item size,
        /// not on the amount of data that is returned to an application. The number of capacity
        /// units consumed will be the same whether you request all of the attributes (the default
        /// behavior) or just some of them (using a projection expression). The number will also
        /// be the same whether or not you use a <code>FilterExpression</code>. 
        /// </para>
        ///  </note> 
        /// <para>
        ///  <code>Query</code> results are always sorted by the sort key value. If the data type
        /// of the sort key is Number, the results are returned in numeric order; otherwise, the
        /// results are returned in order of UTF-8 bytes. By default, the sort order is ascending.
        /// To reverse the order, set the <code>ScanIndexForward</code> parameter to false. 
        /// </para>
        ///  
        /// <para>
        ///  A single <code>Query</code> operation will read up to the maximum number of items
        /// set (if using the <code>Limit</code> parameter) or a maximum of 1 MB of data and then
        /// apply any filtering to the results using <code>FilterExpression</code>. If <code>LastEvaluatedKey</code>
        /// is present in the response, you will need to paginate the result set. For more information,
        /// see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Query.html#Query.Pagination">Paginating
        /// the Results</a> in the <i>Amazon DynamoDB Developer Guide</i>. 
        /// </para>
        ///  
        /// <para>
        ///  <code>FilterExpression</code> is applied after a <code>Query</code> finishes, but
        /// before the results are returned. A <code>FilterExpression</code> cannot contain partition
        /// key or sort key attributes. You need to specify those attributes in the <code>KeyConditionExpression</code>.
        /// 
        /// </para>
        ///  <note> 
        /// <para>
        ///  A <code>Query</code> operation can return an empty result set and a <code>LastEvaluatedKey</code>
        /// if all the items read for the page of results are filtered out. 
        /// </para>
        ///  </note> 
        /// <para>
        /// You can query a table, a local secondary index, or a global secondary index. For a
        /// query on a table or on a local secondary index, you can set the <code>ConsistentRead</code>
        /// parameter to <code>true</code> and obtain a strongly consistent result. Global secondary
        /// indexes support eventually consistent reads only, so do not specify <code>ConsistentRead</code>
        /// when querying a global secondary index.
        /// </para>
        /// </summary>
        /// <param name="request">Container for the necessary parameters to execute the Query service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the Query service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/Query">REST API Reference for Query Operation</seealso>
        public virtual Task<QueryResponse> QueryAsync(QueryRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = QueryRequestMarshaller.Instance,
                ResponseUnmarshaller = QueryResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "Query",
            };

            return InvokeAsync<QueryResponse>(request, options, cancellationToken);
        }

        #endregion
        #region  UpdateItem

        /// <summary>
        /// Edits an existing item's attributes, or adds a new item to the table if it does not
        /// already exist. You can put, delete, or add attribute values. You can also perform
        /// a conditional update on an existing item (insert a new attribute name-value pair if
        /// it doesn't exist, or replace an existing name-value pair if it has certain expected
        /// attribute values).
        /// 
        ///  
        /// <para>
        /// You can also return the item's attribute values in the same <code>UpdateItem</code>
        /// operation using the <code>ReturnValues</code> parameter.
        /// </para>
        /// </summary>
        /// <param name="request">Container for the necessary parameters to execute the UpdateItem service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the UpdateItem service method, as returned by DynamoDB.</returns>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/dynamodb-2012-08-10/UpdateItem">REST API Reference for UpdateItem Operation</seealso>
        public virtual Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = UpdateItemRequestMarshaller.Instance,
                ResponseUnmarshaller = UpdateItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "UpdateItem",
            };

            return InvokeAsync<UpdateItemResponse>(request, options, cancellationToken);
        }

        #endregion
    }
}
