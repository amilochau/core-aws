using Milochau.Core.Aws.DynamoDB.Helpers;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of a <c>DeleteItem</c> operation.
    /// </summary>
    public class DeleteItemResponse : AmazonDynamoDBResponse
    {
        /// <summary>
        /// Attributes
        /// <para>
        /// A map of attribute names to <c>AttributeValue</c> objects, representing the
        /// item as it appeared before the <c>DeleteItem</c> operation. This map appears
        /// in the response only if <c>ReturnValues</c> was specified as <c>ALL_OLD</c>
        /// in the request.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Attributes { get; set; }

        /// <summary>
        /// Item collection metrics
        /// <para>
        /// Information about item collections, if any, that were affected by the <c>DeleteItem</c>
        /// operation. <c>ItemCollectionMetrics</c> is only returned if the <c>ReturnItemCollectionMetrics</c>
        /// parameter was specified. If the table does not have any local secondary indexes, this
        /// information is not returned in the response.
        /// </para>
        /// <para>
        /// Each <c>ItemCollectionMetrics</c> element consists of:
        /// </para>
        /// <list type="table">
        /// <item><term>ItemCollectionKey</term><description>
        /// The partition key value of the item collection.
        /// This is the same as the partition key value of the item itself.
        /// </description></item>
        /// <item><term>SizeEstimateRangeGB</term><description>
        /// <para>
        /// An estimate of item collection size, in gigabytes.
        /// This value is a two-element array containing a lower bound and an upper bound for
        /// the estimate. The estimate includes the size of all the items in the table, plus the
        /// size of all attributes projected into all of the local secondary indexes on that table.
        /// Use this estimate to measure whether a local secondary index is approaching its size
        /// limit.
        /// </para>
        /// <para>
        /// The estimate is subject to change over time; therefore, do not rely on the precision
        /// or accuracy of the estimate.
        /// </para>
        /// </description></item>
        /// </list>
        /// </summary>
        public ItemCollectionMetrics? ItemCollectionMetrics { get; set; }
    }

    /// <inheritdoc/>
    public class DeleteItemResponse<TEntity> : DeleteItemResponse
        where TEntity: class, IDynamoDbDeletableEntity<TEntity>
    {
        /// <inheritdoc/>
        public DeleteItemResponse(DeleteItemResponse response)
        {
            ConsumedCapacity = response.ConsumedCapacity;
            HttpStatusCode = response.HttpStatusCode;
            ResponseMetadata = response.ResponseMetadata;

            Attributes = response.Attributes;
            ItemCollectionMetrics = response.ItemCollectionMetrics;

            Entity = response.Attributes == null ? null : TEntity.ParseFromDynamoDb(response.Attributes);
        }

        /// <summary>Parsed old entity</summary>
        public TEntity? Entity { get; private set; }
    }
}
