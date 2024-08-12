using Milochau.Core.Aws.DynamoDB.Helpers;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents a request to perform a <c>DeleteItem</c> operation on an item.
    /// </summary>
    public class DeleteRequest
    {
        /// <summary>
        /// Key
        /// <para>
        /// A map of attribute name to attribute values, representing the primary key of the item
        /// to delete. All of the table's primary key attributes must be specified, and their
        /// data types must match those of the table's key schema.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Key { get; set; }
    }

    /// <inheritdoc cref="DeleteRequest"/>
    public class DeleteRequest<TEntity> : WriteRequest<TEntity>
        where TEntity : class, IDynamoDbBatchWritableEntity<TEntity>
    {
        /// <summary>Partition key</summary>
        public required AttributeValue PartitionKey { get; set; }

        /// <summary>Sort key</summary>
        public required AttributeValue? SortKey { get; set; }

        internal override WriteRequest Build()
        {
            return new WriteRequest
            {
                DeleteRequest = new DeleteRequest
                {
                    Key = TEntity.SortKey != null && SortKey != null
                        ? new Dictionary<string, AttributeValue>
                        {
                            [TEntity.PartitionKey] = PartitionKey,
                            [TEntity.SortKey] = SortKey,
                        }
                        : new Dictionary<string, AttributeValue>
                        {
                            [TEntity.PartitionKey] = PartitionKey,
                        }
                }
            };
        }
    }
}
