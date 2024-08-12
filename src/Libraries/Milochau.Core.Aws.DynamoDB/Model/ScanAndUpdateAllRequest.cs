using Milochau.Core.Aws.DynamoDB.Helpers;
using System;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>Container for the parameters to the ScanAndUpdateAll operation.</summary>
    public class ScanAndUpdateAllRequest<TScanableEntity, TUpdatableEntity> : ScanAllRequest<TScanableEntity>
        where TScanableEntity : class, IDynamoDbScanableEntity<TScanableEntity>
        where TUpdatableEntity : class, IDynamoDbUpdatableEntity<TUpdatableEntity>
    {
        /// <summary>Function to create an <see cref="UpdateItemRequest{TEntity}"/> from a <typeparamref name="TScanableEntity"/></summary>
        public required Func<TScanableEntity, UpdateItemRequest<TUpdatableEntity>> UpdateItemRequestFunction { get; set; }
    }
}
