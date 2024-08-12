using Milochau.Core.Aws.DynamoDB.Helpers;
using System;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>Container for the parameters to the QueryAndWriteAll operation.</summary>
    public class QueryAndWriteAllRequest<TQueryableEntity, TWritableEntity> : QueryAllRequest<TQueryableEntity>
        where TQueryableEntity : class, IDynamoDbQueryableEntity<TQueryableEntity>
        where TWritableEntity : class, IDynamoDbBatchWritableEntity<TWritableEntity>
    {
        /// <summary>Function to create an <see cref="WriteRequest{TEntity}"/> from a <typeparamref name="TQueryableEntity"/></summary>
        public required Func<TQueryableEntity, WriteRequest<TWritableEntity>> WriteRequestFunction { get; set; }

        /// <inheritdoc cref="BatchWriteItemRequest{TEntity}.ReturnItemCollectionMetrics"/>
        public ReturnItemCollectionMetrics? ReturnItemCollectionMetrics { get; set; }
    }
}
