using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.DynamoDB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Container for the parameters to the GetItem operation.
    /// The <c>GetItem</c> operation returns a set of attributes for the item with the
    /// given primary key. If there is no matching item, <c>GetItem</c> does not return
    /// any data and there will be no <c>Item</c> element in the response.
    /// <para>
    ///  <c>GetItem</c> provides an eventually consistent read by default. If your application
    /// requires a strongly consistent read, set <c>ConsistentRead</c> to <c>true</c>.
    /// Although a strongly consistent read might take more time than an eventually consistent
    /// read, it always returns the last updated value.
    /// </para>
    /// </summary>
    public class GetItemRequest(Guid? userId) : AmazonDynamoDBRequest(userId)
    {
        /// <summary>
        /// Table name
        /// <para>
        /// The name of the table containing the requested item.
        /// </para>
        /// </summary>
        public required string TableName { get; set; }

        /// <summary>
        /// Property key
        /// <para>
        /// A map of attribute names to <c>AttributeValue</c> objects, representing the
        /// primary key of the item to retrieve.
        /// </para>
        /// <para>
        /// For the primary key, you must provide all of the attributes. For example, with a simple
        /// primary key, you only need to provide a value for the partition key. For a composite
        /// primary key, you must provide values for both the partition key and the sort key.
        /// </para>
        /// </summary>
        public required Dictionary<string, AttributeValue> Key { get; set; }

        /// <summary>
        /// Consistent read
        /// <para>
        /// Determines the read consistency model: If set to <c>true</c>, then the operation
        /// uses strongly consistent reads; otherwise, the operation uses eventually consistent
        /// reads.
        /// </para>
        /// </summary>
        public bool? ConsistentRead { get; set; }

        /// <summary>
        /// Expression attribute names
        /// <para>
        /// One or more substitution tokens for attribute names in an expression. The following
        /// are some use cases for using <c>ExpressionAttributeNames</c>:
        /// </para>
        /// <list type="bullet">
        /// <item><description>
        /// To access an attribute whose name conflicts with a DynamoDB reserved word.
        /// </description></item>
        /// <item><description>
        /// To create a placeholder for repeating occurrences of an attribute name in an expression.
        /// </description></item>
        /// <item><description>
        /// To prevent special characters in an attribute name from being misinterpreted in an
        /// expression.
        /// </description></item>
        /// </list>
        /// 
        /// <para>
        /// Use the <b>#</b> character in an expression to dereference an attribute name. For
        /// example, consider the following attribute name:
        /// </para>
        /// <code>Percentile</code>
        /// <para>
        /// The name of this attribute conflicts with a reserved word, so it cannot be used directly
        /// in an expression. (For the complete list of reserved words, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ReservedWords.html">Reserved
        /// Words</a> in the <i>Amazon DynamoDB Developer Guide</i>). To work around this, you
        /// could specify the following for <c>ExpressionAttributeNames</c>:
        /// </para>
        /// <code>{"#P":"Percentile"}</code>
        /// <para>
        /// You could then use this substitution in an expression, as in this example:
        /// </para>
        /// <code>#P = :val</code>
        /// <para>
        /// Tokens that begin with the <b>:</b> character are <i>expression attribute values</i>,
        /// which are placeholders for the actual value at runtime.
        /// </para>
        /// <para>
        /// For more information on expression attribute names, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.AccessingItemAttributes.html">Specifying
        /// Item Attributes</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, string>? ExpressionAttributeNames { get; set; }

        /// <summary>
        /// Projection expression
        /// <para>
        /// A string that identifies one or more attributes to retrieve from the table. These
        /// attributes can include scalars, sets, or elements of a JSON document. The attributes
        /// in the expression must be separated by commas.
        /// </para>
        /// <para>
        /// If no attribute names are specified, then all attributes are returned. If any of the
        /// requested attributes are not found, they do not appear in the result.
        /// </para>
        /// <para>
        /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.AccessingItemAttributes.html">Specifying
        /// Item Attributes</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? ProjectionExpression { get; set; }

        /// <summary>Get request parameters for XRay</summary>
        public override Dictionary<string, object?> GetXRayRequestParameters()
        {
            return new Dictionary<string, object?>
            {
                { "table_name", TableName },
                { "consistent_read", ConsistentRead.ToString() },
                { "projection_expression", ProjectionExpression },
            };
        }
    }

    /// <inheritdoc cref="GetItemRequest"/>
    public class GetItemRequest<TEntity>
        where TEntity : class, IDynamoDbGettableEntity<TEntity>
    {
        /// <inheritdoc cref="AmazonWebServiceRequest.UserId"/>
        public required Guid? UserId { get; set; }

        /// <inheritdoc cref="AmazonDynamoDBRequest.ReturnConsumedCapacity"/>
        public ReturnConsumedCapacity? ReturnConsumedCapacity { get; set; }

        /// <summary>The value of the partition key of the item to retrieve</summary>
        public required AttributeValue PartitionKey { get; set; }

        /// <summary>The value of the sort key of the item to retrieve</summary>
        public required AttributeValue? SortKey { get; set; }

        /// <inheritdoc cref="GetItemRequest.ConsistentRead"/>
        public bool ConsistentRead { get; set; }

        internal GetItemRequest Build()
        {
            List<KeyValuePair<string, AttributeValue>> key = [
                new KeyValuePair<string, AttributeValue>(TEntity.PartitionKey, PartitionKey), // Partition key
            ];

            if (TEntity.SortKey != null && SortKey != null && SortKey.IsSet())
            {
                key.Add(new KeyValuePair<string, AttributeValue>(TEntity.SortKey, SortKey)); // Sort key
            }

            var projectedAttributes = TEntity.ProjectedAttributes;

            return new GetItemRequest(UserId)
            {
                ReturnConsumedCapacity = ReturnConsumedCapacity,

                TableName = TEntity.TableName,
                Key = key.ToDictionary(),
                ConsistentRead = ConsistentRead,
                ProjectionExpression = projectedAttributes == null ? null : new StringBuilder().AppendJoin(", ", projectedAttributes.Select(x => $"#{x}")).ToString(),
                ExpressionAttributeNames = projectedAttributes?.Select(x => new KeyValuePair<string, string>($"#{x}", x)).ToDictionary(),
            };
        }
    }
}
