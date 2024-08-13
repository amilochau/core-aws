using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>Represents the output of a <c>ScanAll</c> operation.</summary>
    public class ScanAllResponse<TEntity>
    {
        /// <summary>Parsed entities</summary>
        public List<TEntity> Entities { get; set; } = [];

        /// <summary>Items</summary>
        public List<Dictionary<string, AttributeValue>> Items { get; set; } = [];

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
        public int Count { get; set; }

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
        public int ScannedCount { get; set; }
    }
}
