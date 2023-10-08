using Amazon.Runtime;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of a <code>Query</code> operation.
    /// </summary>
    public partial class QueryResponse : AmazonWebServiceResponse
    {
        /// <summary>
        /// Gets and sets the property ConsumedCapacity. 
        /// <para>
        /// The capacity units consumed by the <code>Query</code> operation. The data returned
        /// includes the total provisioned throughput consumed, along with statistics for the
        /// table and any indexes involved in the operation. <code>ConsumedCapacity</code> is
        /// only returned if the <code>ReturnConsumedCapacity</code> parameter was specified.
        /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ProvisionedThroughputIntro.html">Provisioned
        /// Throughput</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public ConsumedCapacity? ConsumedCapacity { get; set; }

        /// <summary>
        /// Gets and sets the property Count. 
        /// <para>
        /// The number of items in the response.
        /// </para>
        ///  
        /// <para>
        /// If you used a <code>QueryFilter</code> in the request, then <code>Count</code> is
        /// the number of items returned after the filter was applied, and <code>ScannedCount</code>
        /// is the number of matching items before the filter was applied.
        /// </para>
        ///  
        /// <para>
        /// If you did not use a filter in the request, then <code>Count</code> and <code>ScannedCount</code>
        /// are the same.
        /// </para>
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Gets and sets the property Items. 
        /// <para>
        /// An array of item attributes that match the query criteria. Each element in this array
        /// consists of an attribute name and the value for that attribute.
        /// </para>
        /// </summary>
        public List<Dictionary<string, AttributeValue>>? Items { get; set; }

        /// <summary>
        /// Gets and sets the property LastEvaluatedKey. 
        /// <para>
        /// The primary key of the item where the operation stopped, inclusive of the previous
        /// result set. Use this value to start a new operation, excluding this value in the new
        /// request.
        /// </para>
        ///  
        /// <para>
        /// If <code>LastEvaluatedKey</code> is empty, then the "last page" of results has been
        /// processed and there is no more data to be retrieved.
        /// </para>
        ///  
        /// <para>
        /// If <code>LastEvaluatedKey</code> is not empty, it does not necessarily mean that there
        /// is more data in the result set. The only way to know when you have reached the end
        /// of the result set is when <code>LastEvaluatedKey</code> is empty.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? LastEvaluatedKey { get; set; }

        /// <summary>
        /// Gets and sets the property ScannedCount. 
        /// <para>
        /// The number of items evaluated, before any <code>QueryFilter</code> is applied. A high
        /// <code>ScannedCount</code> value with few, or no, <code>Count</code> results indicates
        /// an inefficient <code>Query</code> operation. For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/QueryAndScan.html#Count">Count
        /// and ScannedCount</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        ///  
        /// <para>
        /// If you did not use a filter in the request, then <code>ScannedCount</code> is the
        /// same as <code>Count</code>.
        /// </para>
        /// </summary>
        public int? ScannedCount { get; set; }
    }
}