using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Container for the parameters to the PutItem operation.
    /// Creates a new item, or replaces an old item with a new item. If an item that has the
    /// same primary key as the new item already exists in the specified table, the new item
    /// completely replaces the existing item. You can perform a conditional put operation
    /// (add a new item if one with the specified primary key doesn't exist), or replace an
    /// existing item if it has certain attribute values. You can return the item's attribute
    /// values in the same operation, using the <c>ReturnValues</c> parameter.
    /// <para>
    /// When you add an item, the primary key attributes are the only required attributes.
    /// </para>
    /// <para>
    /// Empty String and Binary attribute values are allowed. Attribute values of type String
    /// and Binary must have a length greater than zero if the attribute is used as a key
    /// attribute for a table or index. Set type attributes cannot be empty. 
    /// </para>
    /// <para>
    /// Invalid Requests with empty values will be rejected with a <c>ValidationException</c>
    /// exception.
    /// </para>
    /// <para>
    /// To prevent a new item from replacing an existing item, use a conditional expression
    /// that contains the <c>attribute_not_exists</c> function with the name of the
    /// attribute being used as the partition key for the table. Since every record must contain
    /// that attribute, the <c>attribute_not_exists</c> function will only succeed if
    /// no matching item exists.
    /// </para>
    /// <para>
    /// For more information about <c>PutItem</c>, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/WorkingWithItems.html">Working
    /// with Items</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </para>
    /// </summary>
    public class PutItemRequest(Guid? userId) : AmazonDynamoDBRequest(userId)
    {
        /// <summary>
        /// Table name
        /// <para>
        /// The name of the table to contain the item.
        /// </para>
        /// </summary>
        public required string TableName { get; set; }

        /// <summary>
        /// Item
        /// <para>
        /// A map of attribute name/value pairs, one for each attribute. Only the primary key
        /// attributes are required; you can optionally provide other attribute name-value pairs
        /// for the item.
        /// </para>
        /// <para>
        /// You must provide all of the attributes for the primary key. For example, with a simple
        /// primary key, you only need to provide a value for the partition key. For a composite
        /// primary key, you must provide both values for both the partition key and the sort
        /// key.
        /// </para>
        /// <para>
        /// If you specify any attributes that are part of an index key, then the data types for
        /// those attributes must match those of the schema in the table's attribute definition.
        /// </para>
        /// <para>
        /// Empty String and Binary attribute values are allowed. Attribute values of type String
        /// and Binary must have a length greater than zero if the attribute is used as a key
        /// attribute for a table or index.
        /// </para>
        /// <para>
        /// For more information about primary keys, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.CoreComponents.html#HowItWorks.CoreComponents.PrimaryKey">Primary
        /// Key</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// <para>
        /// Each element in the <c>Item</c> map is an <c>AttributeValue</c> object.
        /// </para>
        /// </summary>
        public required Dictionary<string, AttributeValue> Item { get; set; }

        /// <summary>
        /// Condition expression
        /// <para>
        /// A condition that must be satisfied in order for a conditional <c>PutItem</c>
        /// operation to succeed.
        /// </para>
        /// <para>
        /// An expression can contain any of the following:
        /// </para>
        /// <list type="bullet">
        /// <item><description>
        /// Functions: <c>attribute_exists | attribute_not_exists | attribute_type | contains
        /// | begins_with | size</c> - These function names are case-sensitive.
        /// </description></item>
        /// <item><description>
        /// Comparison operators: <c>= | &lt;&gt; | &lt; | &gt; | &lt;= | &gt;= | BETWEEN |
        /// IN </c> 
        /// </description></item>
        /// <item><description>
        /// Logical operators: <c>AND | OR | NOT</c> 
        /// </description></item>
        /// </list>
        /// <para>
        /// For more information on condition expressions, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Condition
        /// Expressions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? ConditionExpression { get; set; }

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
        /// Expression attribute values
        /// <para>
        /// One or more values that can be substituted in an expression.
        /// </para>
        /// <para>
        /// Use the <b>:</b> (colon) character in an expression to dereference an attribute value.
        /// For example, suppose that you wanted to check whether the value of the <i>ProductStatus</i>
        /// attribute was one of the following: 
        /// </para>
        /// <code>Available | Backordered | Discontinued</code>
        /// <para>
        /// You would first need to specify <c>ExpressionAttributeValues</c> as follows:
        /// </para>
        /// <code>{ ":avail":{"S":"Available"}, ":back":{"S":"Backordered"}, ":disc":{"S":"Discontinued"}
        /// }</code>
        /// <para>
        /// You could then use these values in an expression, such as this:
        /// </para>
        /// <code>ProductStatus IN (:avail, :back, :disc)</code>
        /// <para>
        /// For more information on expression attribute values, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Condition
        /// Expressions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? ExpressionAttributeValues { get; set; }

        /// <summary>
        /// Return item collection metrics
        /// <para>
        /// Determines whether item collection metrics are returned. If set to <c>SIZE</c>,
        /// the response includes statistics about item collections, if any, that were modified
        /// during the operation are returned in the response. If set to <c>NONE</c> (the
        /// default), no statistics are returned.
        /// </para>
        /// </summary>
        public ReturnItemCollectionMetrics? ReturnItemCollectionMetrics { get; set; }

        /// <summary>
        /// Return values
        /// <para>
        /// Use <c>ReturnValues</c> if you want to get the item attributes as they appeared
        /// before they were updated with the <c>PutItem</c> request. For <c>PutItem</c>,
        /// the valid values are:
        /// </para>
        /// <list type="table">
        /// <item><term>NONE</term><description>
        /// If <c>ReturnValues</c> is not specified, or if its value
        /// is <c>NONE</c>, then nothing is returned. (This setting is the default for <c>ReturnValues</c>.)
        /// </description></item>
        /// <item><term>ALL_OLD</term><description>
        /// If <c>PutItem</c> overwrote an attribute name-value
        /// pair, then the content of the old item is returned.
        /// </description></item>
        /// </list>
        /// <para>
        /// The values returned are strongly consistent.
        /// </para>
        /// <para>
        /// There is no additional cost associated with requesting a return value aside from the
        /// small network and processing overhead of receiving a larger response. No read capacity
        /// units are consumed.
        /// </para>
        /// <para>
        /// The <c>ReturnValues</c> parameter is used by several DynamoDB operations; however,
        /// <c>PutItem</c> does not recognize any values other than <c>NONE</c> or
        /// <c>ALL_OLD</c>.
        /// </para>
        /// </summary>
        public ReturnValue? ReturnValues { get; set; }

        /// <summary>
        /// Return values on condition check failure
        /// <para>
        /// An optional parameter that returns the item attributes for a <c>PutItem</c>
        /// operation that failed a condition check.
        /// </para>
        /// <para>
        /// There is no additional cost associated with requesting a return value aside from the
        /// small network and processing overhead of receiving a larger response. No read capacity
        /// units are consumed.
        /// </para>
        /// </summary>
        public ReturnValuesOnConditionCheckFailure? ReturnValuesOnConditionCheckFailure { get; set; }

        /// <summary>Get request parameters for XRay</summary>
        public override Dictionary<string, object?> GetXRayRequestParameters()
        {
            return new Dictionary<string, object?>
            {
                { "table_name", TableName },
            };
        }
    }

    /// <inheritdoc cref="PutItemRequest"/>
    public class PutItemRequest<TEntity>
        where TEntity : class, IDynamoDbPutableEntity<TEntity>
    {
        /// <inheritdoc cref="AmazonWebServiceRequest.UserId"/>
        public required Guid? UserId { get; set; }

        /// <inheritdoc cref="AmazonDynamoDBRequest.ReturnConsumedCapacity"/>
        public ReturnConsumedCapacity? ReturnConsumedCapacity { get; set; }


        /// <inheritdoc cref="PutItemRequest.Item"/>
        public required TEntity Entity { get; set; }


        /// <summary>
        /// A condition that must be satisfied in order for a conditional <c>PutItem</c>
        /// operation to succeed.
        /// <para>
        /// The following implementations can be used:
        /// <list type="table">
        /// <item><term>Comparisons</term><description>
        /// <see cref="EqualExpression"/>, <see cref="NotEqualExpression"/>, <see cref="ComparatorExpression"/>, <see cref="BetweenExpression"/>, <see cref="InExpression"/>
        /// </description></item>
        /// <item><term>Functions</term><description>
        /// <see cref="AttributeExistsExpression"/>, <see cref="AttributeNotExistsExpression"/>, <see cref="AttributeTypeExpression"/>, <see cref="BeginsWithExpression"/>, <see cref="ContainsExpression"/>
        /// </description></item>
        /// <item><term>Logical evaluations</term><description>
        /// <see cref="AndExpression"/>, <see cref="OrExpression"/>, <see cref="NotExpression"/>
        /// </description></item>
        /// <item><term>Parentheses</term><description>
        /// <see cref="ParenthesesExpression"/>
        /// </description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <remarks>Not implemented: <see cref="SizeExpression"/></remarks>
        public IFilterExpression? Conditions { get; set; }


        /// <inheritdoc cref="PutItemRequest.ReturnItemCollectionMetrics"/>
        public ReturnItemCollectionMetrics? ReturnItemCollectionMetrics { get; set; }

        /// <inheritdoc cref="PutItemRequest.ReturnValues"/>
        public ReturnValue? ReturnValues { get; set; }

        /// <inheritdoc cref="PutItemRequest.ReturnValuesOnConditionCheckFailure"/>
        public ReturnValuesOnConditionCheckFailure? ReturnValuesOnConditionCheckFailure { get; set; }

        internal PutItemRequest Build()
        {
            return new PutItemRequest(UserId)
            {
                ReturnConsumedCapacity = ReturnConsumedCapacity,

                TableName = TEntity.TableName,
                Item = Entity.FormatForDynamoDb().Where(x => x.Value.IsSet()).ToDictionary(),

                ConditionExpression = Conditions?.Expression,
                ExpressionAttributeNames = Conditions?.AttributeNames.ToDictionary(),
                ExpressionAttributeValues = Conditions?.AttributeValues.ToDictionary(),
                ReturnItemCollectionMetrics = ReturnItemCollectionMetrics,
                ReturnValues = ReturnValues,
                ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure,
            };
        }
    }
}