using Milochau.Core.Aws.DynamoDB.Helpers;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of a <c>GetItem</c> operation.
    /// </summary>
    public class GetItemResponse : AmazonDynamoDBResponse
    {
        /// <summary>
        /// Item
        /// <para>
        /// A map of attribute names to <c>AttributeValue</c> objects, as specified by <c>ProjectionExpression</c>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Item { get; set; }
    }

    /// <inheritdoc/>
    public class GetItemResponse<TEntity> : GetItemResponse
        where TEntity : class, IDynamoDbGettableEntity<TEntity>
    {
        /// <inheritdoc/>
        public GetItemResponse(GetItemResponse response)
        {
            ConsumedCapacity = response.ConsumedCapacity;
            HttpStatusCode = response.HttpStatusCode;
            ResponseMetadata = response.ResponseMetadata;

            Item = response.Item;

            Entity = response.Item == null ? null : TEntity.ParseFromDynamoDb(response.Item);
        }

        /// <summary>Parsed entity</summary>
        public TEntity? Entity { get; private set; }
    }
}