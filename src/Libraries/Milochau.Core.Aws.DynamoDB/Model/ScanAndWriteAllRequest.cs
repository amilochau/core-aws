using Milochau.Core.Aws.DynamoDB.Helpers;
using System;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>Container for the parameters to the ScanAndWriteAll operation.</summary>
    public class ScanAndWriteAllRequest<TScanableEntity, TWritableEntity> : ScanAllRequest<TScanableEntity>
        where TScanableEntity : class, IDynamoDbScanableEntity<TScanableEntity>
        where TWritableEntity : class, IDynamoDbBatchWritableEntity<TWritableEntity>
    {
        /// <summary>Function to create an <see cref="WriteRequest{TEntity}"/> from a <typeparamref name="TScanableEntity"/></summary>
        public required Func<TScanableEntity, WriteRequest<TWritableEntity>> WriteRequestFunction { get; set; }

        /// <inheritdoc cref="BatchWriteItemRequest{TEntity}.ReturnItemCollectionMetrics"/>
        public ReturnItemCollectionMetrics? ReturnItemCollectionMetrics { get; set; }
    }
}
