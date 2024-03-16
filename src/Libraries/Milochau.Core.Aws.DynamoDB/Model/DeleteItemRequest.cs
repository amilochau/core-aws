using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Container for the parameters to the DeleteItem operation.
    /// Deletes a single item in a table by primary key. You can perform a conditional delete
    /// operation that deletes the item if it exists, or if it has an expected attribute value.
    /// 
    ///  
    /// <para>
    /// In addition to deleting an item, you can also return the item's attribute values in
    /// the same operation, using the <c>ReturnValues</c> parameter.
    /// </para>
    ///  
    /// <para>
    /// Unless you specify conditions, the <c>DeleteItem</c> is an idempotent operation;
    /// running it multiple times on the same item or attribute does <i>not</i> result in
    /// an error response.
    /// </para>
    ///  
    /// <para>
    /// Conditional deletes are useful for deleting items only if specific conditions are
    /// met. If those conditions are met, DynamoDB performs the delete. Otherwise, the item
    /// is not deleted.
    /// </para>
    /// </summary>
    public class DeleteItemRequest(Guid? userId) : AmazonDynamoDBRequest(userId)
    {
        /// <summary>
        /// Gets and sets the property ConditionalOperator. 
        /// <para>
        /// This is a legacy parameter. Use <c>ConditionExpression</c> instead. For more
        /// information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LegacyConditionalParameters.ConditionalOperator.html">ConditionalOperator</a>
        /// in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public ConditionalOperator? ConditionalOperator { get; set; }

        /// <summary>
        /// Gets and sets the property ConditionExpression. 
        /// <para>
        /// A condition that must be satisfied in order for a conditional <c>DeleteItem</c>
        /// to succeed.
        /// </para>
        ///  
        /// <para>
        /// An expression can contain any of the following:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// Functions: <c>attribute_exists | attribute_not_exists | attribute_type | contains
        /// | begins_with | size</c> 
        /// </para>
        ///  
        /// <para>
        /// These function names are case-sensitive.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Comparison operators: <c>= | &lt;&gt; | &lt; | &gt; | &lt;= | &gt;= | BETWEEN |
        /// IN </c> 
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  Logical operators: <c>AND | OR | NOT</c> 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// For more information about condition expressions, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Condition
        /// Expressions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? ConditionExpression { get; set; }

        /// <summary>
        /// Gets and sets the property Expected. 
        /// <para>
        /// This is a legacy parameter. Use <c>ConditionExpression</c> instead. For more
        /// information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LegacyConditionalParameters.Expected.html">Expected</a>
        /// in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, ExpectedAttributeValue>? Expected { get; set; }

        /// <summary>
        /// Gets and sets the property ExpressionAttributeNames. 
        /// <para>
        /// One or more substitution tokens for attribute names in an expression. The following
        /// are some use cases for using <c>ExpressionAttributeNames</c>:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// To access an attribute whose name conflicts with a DynamoDB reserved word.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// To create a placeholder for repeating occurrences of an attribute name in an expression.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// To prevent special characters in an attribute name from being misinterpreted in an
        /// expression.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// Use the <b>#</b> character in an expression to dereference an attribute name. For
        /// example, consider the following attribute name:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>Percentile</c> 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// The name of this attribute conflicts with a reserved word, so it cannot be used directly
        /// in an expression. (For the complete list of reserved words, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ReservedWords.html">Reserved
        /// Words</a> in the <i>Amazon DynamoDB Developer Guide</i>). To work around this, you
        /// could specify the following for <c>ExpressionAttributeNames</c>:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>{"#P":"Percentile"}</c> 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// You could then use this substitution in an expression, as in this example:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>#P = :val</c> 
        /// </para>
        ///  </li> </ul> <note> 
        /// <para>
        /// Tokens that begin with the <b>:</b> character are <i>expression attribute values</i>,
        /// which are placeholders for the actual value at runtime.
        /// </para>
        ///  </note> 
        /// <para>
        /// For more information on expression attribute names, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.AccessingItemAttributes.html">Specifying
        /// Item Attributes</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, string>? ExpressionAttributeNames { get; set; }

        /// <summary>
        /// Gets and sets the property ExpressionAttributeValues. 
        /// <para>
        /// One or more values that can be substituted in an expression.
        /// </para>
        ///  
        /// <para>
        /// Use the <b>:</b> (colon) character in an expression to dereference an attribute value.
        /// For example, suppose that you wanted to check whether the value of the <i>ProductStatus</i>
        /// attribute was one of the following: 
        /// </para>
        ///  
        /// <para>
        ///  <c>Available | Backordered | Discontinued</c> 
        /// </para>
        ///  
        /// <para>
        /// You would first need to specify <c>ExpressionAttributeValues</c> as follows:
        /// </para>
        ///  
        /// <para>
        ///  <c>{ ":avail":{"S":"Available"}, ":back":{"S":"Backordered"}, ":disc":{"S":"Discontinued"}
        /// }</c> 
        /// </para>
        ///  
        /// <para>
        /// You could then use these values in an expression, such as this:
        /// </para>
        ///  
        /// <para>
        ///  <c>ProductStatus IN (:avail, :back, :disc)</c> 
        /// </para>
        ///  
        /// <para>
        /// For more information on expression attribute values, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Condition
        /// Expressions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? ExpressionAttributeValues { get; set; }

        /// <summary>
        /// Gets and sets the property Key. 
        /// <para>
        /// A map of attribute names to <c>AttributeValue</c> objects, representing the
        /// primary key of the item to delete.
        /// </para>
        ///  
        /// <para>
        /// For the primary key, you must provide all of the key attributes. For example, with
        /// a simple primary key, you only need to provide a value for the partition key. For
        /// a composite primary key, you must provide values for both the partition key and the
        /// sort key.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Key { get; set; }

        /// <summary>
        /// Gets and sets the property ReturnItemCollectionMetrics. 
        /// <para>
        /// Determines whether item collection metrics are returned. If set to <c>SIZE</c>,
        /// the response includes statistics about item collections, if any, that were modified
        /// during the operation are returned in the response. If set to <c>NONE</c> (the
        /// default), no statistics are returned.
        /// </para>
        /// </summary>
        public ReturnItemCollectionMetrics? ReturnItemCollectionMetrics { get; set; }

        /// <summary>
        /// Gets and sets the property ReturnValues. 
        /// <para>
        /// Use <c>ReturnValues</c> if you want to get the item attributes as they appeared
        /// before they were deleted. For <c>DeleteItem</c>, the valid values are:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>NONE</c> - If <c>ReturnValues</c> is not specified, or if its value
        /// is <c>NONE</c>, then nothing is returned. (This setting is the default for <c>ReturnValues</c>.)
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>ALL_OLD</c> - The content of the old item is returned.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// There is no additional cost associated with requesting a return value aside from the
        /// small network and processing overhead of receiving a larger response. No read capacity
        /// units are consumed.
        /// </para>
        ///  <note> 
        /// <para>
        /// The <c>ReturnValues</c> parameter is used by several DynamoDB operations; however,
        /// <c>DeleteItem</c> does not recognize any values other than <c>NONE</c>
        /// or <c>ALL_OLD</c>.
        /// </para>
        ///  </note>
        /// </summary>
        public ReturnValue? ReturnValues { get; set; }

        /// <summary>
        /// Gets and sets the property ReturnValuesOnConditionCheckFailure. 
        /// <para>
        /// An optional parameter that returns the item attributes for a <c>DeleteItem</c>
        /// operation that failed a condition check.
        /// </para>
        ///  
        /// <para>
        /// There is no additional cost associated with requesting a return value aside from the
        /// small network and processing overhead of receiving a larger response. No read capacity
        /// units are consumed.
        /// </para>
        /// </summary>
        public ReturnValuesOnConditionCheckFailure? ReturnValuesOnConditionCheckFailure { get; set; }

        /// <summary>
        /// Gets and sets the property TableName. 
        /// <para>
        /// The name of the table from which to delete the item.
        /// </para>
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>Get request parameters for XRay</summary>
        public override Dictionary<string, object?> GetXRayRequestParameters()
        {
            return new Dictionary<string, object?>
            {
                { "table_name", TableName },
            };
        }
    }
}