using Milochau.Core.Aws.DynamoDB.Helpers;
using System;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>Container for the parameters to the QueryAndUpdateAll operation.</summary>
    public class QueryAndUpdateAllRequest<TQueryableEntity, TUpdatableEntity> : QueryAllRequest<TQueryableEntity>
        where TQueryableEntity : class, IDynamoDbQueryableEntity<TQueryableEntity>
        where TUpdatableEntity : class, IDynamoDbUpdatableEntity<TUpdatableEntity>
    {
        /// <summary>Function to create an <see cref="UpdateItemRequest{TEntity}"/> from a <typeparamref name="TQueryableEntity"/></summary>
        public required Func<TQueryableEntity, UpdateItemRequest<TUpdatableEntity>> UpdateItemRequestFunction { get; set; }
    }
}
