using Milochau.Core.Aws.DynamoDB.Helpers;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents a request to perform a <c>PutItem</c> operation on an item.
    /// </summary>
    public class PutRequest
    {
        /// <summary>
        /// Item
        /// <para>
        /// A map of attribute name to attribute values, representing the primary key of an item
        /// to be processed by <c>PutItem</c>. All of the table's primary key attributes
        /// must be specified, and their data types must match those of the table's key schema.
        /// If any attributes are present in the item that are part of an index key schema for
        /// the table, their types must match the index key schema.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Item { get; set; }
    }

    /// <inheritdoc cref="PutRequest"/>
    public class PutRequest<TEntity> : WriteRequest<TEntity>
        where TEntity : IDynamoDbFormattableEntity
    {
        /// <summary>Entity to put</summary>
        public required TEntity Entity { get; set; }

        internal override WriteRequest Build()
        {
            return new WriteRequest
            {
                PutRequest = new PutRequest
                {
                    Item = Entity.FormatForDynamoDb(),
                },
            };
        }
    }
}
