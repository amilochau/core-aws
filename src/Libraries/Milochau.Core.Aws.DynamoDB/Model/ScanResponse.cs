using Milochau.Core.Aws.DynamoDB.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of a <c>Scan</c> operation.
    /// </summary>
    public partial class ScanResponse : AmazonDynamoDBResponse
    {
        /// <summary>
        /// Count
        /// <para>
        /// The number of items in the response.
        /// </para>
        /// <para>
        /// If you set <c>ScanFilter</c> in the request, then <c>Count</c> is the number of items
        /// returned after the filter was applied, and <c>ScannedCount</c> is the number of matching
        /// items before the filter was applied.
        /// </para>
        /// <para>
        /// If you did not use a filter in the request, then <c>Count</c> is the same as <c>ScannedCount</c>.
        /// </para>
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Items
        /// <para>
        /// An array of item attributes that match the scan criteria. Each element in this array
        /// consists of an attribute name and the value for that attribute.
        /// </para>
        /// </summary>
        public List<Dictionary<string, AttributeValue>>? Items { get; set; }

        /// <summary>
        /// Last evaluated key
        /// <para>
        /// The primary key of the item where the operation stopped, inclusive of the previous
        /// result set. Use this value to start a new operation, excluding this value in the new
        /// request.
        /// </para>
        /// <para>
        /// If <c>LastEvaluatedKey</c> is empty, then the "last page" of results has been processed
        /// and there is no more data to be retrieved.
        /// </para>
        /// <para>
        /// If <c>LastEvaluatedKey</c> is not empty, it does not necessarily mean that there is
        /// more data in the result set. The only way to know when you have reached the end of
        /// the result set is when <c>LastEvaluatedKey</c> is empty.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? LastEvaluatedKey { get; set; }

        /// <summary>
        /// Scanned count
        /// <para>
        /// The number of items evaluated, before any <c>ScanFilter</c> is applied. A high <c>ScannedCount</c>
        /// value with few, or no, <c>Count</c> results indicates an inefficient <c>Scan</c> operation.
        /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/QueryAndScan.html#Count">Count
        /// and ScannedCount</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// <para>
        /// If you did not use a filter in the request, then <c>ScannedCount</c> is the same as
        /// <c>Count</c>.
        /// </para>
        /// </summary>
        public int? ScannedCount { get; set; }
    }

    /// <inheritdoc/>
    public class ScanResponse<TEntity> : ScanResponse
        where TEntity : class, IDynamoDbScanableEntity<TEntity>
    {
        /// <inheritdoc/>
        public ScanResponse(ScanResponse response)
        {
            ConsumedCapacity = response.ConsumedCapacity;
            HttpStatusCode = response.HttpStatusCode;
            ResponseMetadata = response.ResponseMetadata;

            Count = response.Count;
            Items = response.Items;
            LastEvaluatedKey = response.LastEvaluatedKey;
            ScannedCount = response.ScannedCount;

            Entities = Items == null || Items.Count == 0 ? null : Items.Select(TEntity.ParseFromDynamoDb).ToList();
        }

        /// <summary>Parsed entities</summary>
        public List<TEntity>? Entities { get; private set; }
    }
}
