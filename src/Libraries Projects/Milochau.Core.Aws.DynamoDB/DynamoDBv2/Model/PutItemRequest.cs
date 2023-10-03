using System.Collections.Generic;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/PutItemRequest.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// Container for the parameters to the PutItem operation.
    /// Creates a new item, or replaces an old item with a new item. If an item that has the
    /// same primary key as the new item already exists in the specified table, the new item
    /// completely replaces the existing item. You can perform a conditional put operation
    /// (add a new item if one with the specified primary key doesn't exist), or replace an
    /// existing item if it has certain attribute values. You can return the item's attribute
    /// values in the same operation, using the <code>ReturnValues</code> parameter.
    /// 
    ///  
    /// <para>
    /// When you add an item, the primary key attributes are the only required attributes.
    /// 
    /// </para>
    ///  
    /// <para>
    /// Empty String and Binary attribute values are allowed. Attribute values of type String
    /// and Binary must have a length greater than zero if the attribute is used as a key
    /// attribute for a table or index. Set type attributes cannot be empty. 
    /// </para>
    ///  
    /// <para>
    /// Invalid Requests with empty values will be rejected with a <code>ValidationException</code>
    /// exception.
    /// </para>
    ///  <note> 
    /// <para>
    /// To prevent a new item from replacing an existing item, use a conditional expression
    /// that contains the <code>attribute_not_exists</code> function with the name of the
    /// attribute being used as the partition key for the table. Since every record must contain
    /// that attribute, the <code>attribute_not_exists</code> function will only succeed if
    /// no matching item exists.
    /// </para>
    ///  </note> 
    /// <para>
    /// For more information about <code>PutItem</code>, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/WorkingWithItems.html">Working
    /// with Items</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </para>
    /// </summary>
    public partial class PutItemRequest : AmazonDynamoDBRequest
    {
        /// <summary>
        /// Gets and sets the property ConditionalOperator. 
        /// <para>
        /// This is a legacy parameter. Use <code>ConditionExpression</code> instead. For more
        /// information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LegacyConditionalParameters.ConditionalOperator.html">ConditionalOperator</a>
        /// in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public ConditionalOperator? ConditionalOperator { get; set; }

        // Check to see if ConditionalOperator property is set
        internal bool IsSetConditionalOperator()
        {
            return ConditionalOperator != null;
        }

        /// <summary>
        /// Gets and sets the property ConditionExpression. 
        /// <para>
        /// A condition that must be satisfied in order for a conditional <code>PutItem</code>
        /// operation to succeed.
        /// </para>
        ///  
        /// <para>
        /// An expression can contain any of the following:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// Functions: <code>attribute_exists | attribute_not_exists | attribute_type | contains
        /// | begins_with | size</code> 
        /// </para>
        ///  
        /// <para>
        /// These function names are case-sensitive.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Comparison operators: <code>= | &lt;&gt; | &lt; | &gt; | &lt;= | &gt;= | BETWEEN |
        /// IN </code> 
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  Logical operators: <code>AND | OR | NOT</code> 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// For more information on condition expressions, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Condition
        /// Expressions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? ConditionExpression { get; set; }

        // Check to see if ConditionExpression property is set
        internal bool IsSetConditionExpression()
        {
            return ConditionExpression != null;
        }

        /// <summary>
        /// Gets and sets the property Expected. 
        /// <para>
        /// This is a legacy parameter. Use <code>ConditionExpression</code> instead. For more
        /// information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LegacyConditionalParameters.Expected.html">Expected</a>
        /// in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, ExpectedAttributeValue> Expected { get; set; } = new Dictionary<string, ExpectedAttributeValue>();

        // Check to see if Expected property is set
        internal bool IsSetExpected()
        {
            return Expected != null && Expected.Count > 0;
        }

        /// <summary>
        /// Gets and sets the property ExpressionAttributeNames. 
        /// <para>
        /// One or more substitution tokens for attribute names in an expression. The following
        /// are some use cases for using <code>ExpressionAttributeNames</code>:
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
        ///  <code>Percentile</code> 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// The name of this attribute conflicts with a reserved word, so it cannot be used directly
        /// in an expression. (For the complete list of reserved words, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ReservedWords.html">Reserved
        /// Words</a> in the <i>Amazon DynamoDB Developer Guide</i>). To work around this, you
        /// could specify the following for <code>ExpressionAttributeNames</code>:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>{"#P":"Percentile"}</code> 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// You could then use this substitution in an expression, as in this example:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>#P = :val</code> 
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
        public Dictionary<string, string> ExpressionAttributeNames { get; set; } = new Dictionary<string, string>();

        // Check to see if ExpressionAttributeNames property is set
        internal bool IsSetExpressionAttributeNames()
        {
            return ExpressionAttributeNames != null && ExpressionAttributeNames.Count > 0;
        }

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
        ///  <code>Available | Backordered | Discontinued</code> 
        /// </para>
        ///  
        /// <para>
        /// You would first need to specify <code>ExpressionAttributeValues</code> as follows:
        /// </para>
        ///  
        /// <para>
        ///  <code>{ ":avail":{"S":"Available"}, ":back":{"S":"Backordered"}, ":disc":{"S":"Discontinued"}
        /// }</code> 
        /// </para>
        ///  
        /// <para>
        /// You could then use these values in an expression, such as this:
        /// </para>
        ///  
        /// <para>
        ///  <code>ProductStatus IN (:avail, :back, :disc)</code> 
        /// </para>
        ///  
        /// <para>
        /// For more information on expression attribute values, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Condition
        /// Expressions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue> ExpressionAttributeValues { get; set; } = new Dictionary<string, AttributeValue>();

        // Check to see if ExpressionAttributeValues property is set
        internal bool IsSetExpressionAttributeValues()
        {
            return ExpressionAttributeValues != null && ExpressionAttributeValues.Count > 0;
        }

        /// <summary>
        /// Gets and sets the property Item. 
        /// <para>
        /// A map of attribute name/value pairs, one for each attribute. Only the primary key
        /// attributes are required; you can optionally provide other attribute name-value pairs
        /// for the item.
        /// </para>
        ///  
        /// <para>
        /// You must provide all of the attributes for the primary key. For example, with a simple
        /// primary key, you only need to provide a value for the partition key. For a composite
        /// primary key, you must provide both values for both the partition key and the sort
        /// key.
        /// </para>
        ///  
        /// <para>
        /// If you specify any attributes that are part of an index key, then the data types for
        /// those attributes must match those of the schema in the table's attribute definition.
        /// </para>
        ///  
        /// <para>
        /// Empty String and Binary attribute values are allowed. Attribute values of type String
        /// and Binary must have a length greater than zero if the attribute is used as a key
        /// attribute for a table or index.
        /// </para>
        ///  
        /// <para>
        /// For more information about primary keys, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.CoreComponents.html#HowItWorks.CoreComponents.PrimaryKey">Primary
        /// Key</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        ///  
        /// <para>
        /// Each element in the <code>Item</code> map is an <code>AttributeValue</code> object.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue> Item { get; set; } = new Dictionary<string, AttributeValue>();

        // Check to see if Item property is set
        internal bool IsSetItem()
        {
            return Item != null && Item.Count > 0;
        }

        /// <summary>
        /// Gets and sets the property ReturnConsumedCapacity.
        /// </summary>
        public ReturnConsumedCapacity? ReturnConsumedCapacity { get; set; }

        // Check to see if ReturnConsumedCapacity property is set
        internal bool IsSetReturnConsumedCapacity()
        {
            return ReturnConsumedCapacity != null;
        }

        /// <summary>
        /// Gets and sets the property ReturnItemCollectionMetrics. 
        /// <para>
        /// Determines whether item collection metrics are returned. If set to <code>SIZE</code>,
        /// the response includes statistics about item collections, if any, that were modified
        /// during the operation are returned in the response. If set to <code>NONE</code> (the
        /// default), no statistics are returned.
        /// </para>
        /// </summary>
        public ReturnItemCollectionMetrics? ReturnItemCollectionMetrics { get; set; }

        // Check to see if ReturnItemCollectionMetrics property is set
        internal bool IsSetReturnItemCollectionMetrics()
        {
            return ReturnItemCollectionMetrics != null;
        }

        /// <summary>
        /// Gets and sets the property ReturnValues. 
        /// <para>
        /// Use <code>ReturnValues</code> if you want to get the item attributes as they appeared
        /// before they were updated with the <code>PutItem</code> request. For <code>PutItem</code>,
        /// the valid values are:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>NONE</code> - If <code>ReturnValues</code> is not specified, or if its value
        /// is <code>NONE</code>, then nothing is returned. (This setting is the default for <code>ReturnValues</code>.)
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>ALL_OLD</code> - If <code>PutItem</code> overwrote an attribute name-value
        /// pair, then the content of the old item is returned.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// The values returned are strongly consistent.
        /// </para>
        ///  
        /// <para>
        /// There is no additional cost associated with requesting a return value aside from the
        /// small network and processing overhead of receiving a larger response. No read capacity
        /// units are consumed.
        /// </para>
        ///  <note> 
        /// <para>
        /// The <code>ReturnValues</code> parameter is used by several DynamoDB operations; however,
        /// <code>PutItem</code> does not recognize any values other than <code>NONE</code> or
        /// <code>ALL_OLD</code>.
        /// </para>
        ///  </note>
        /// </summary>
        public ReturnValue? ReturnValues { get; set; }

        // Check to see if ReturnValues property is set
        internal bool IsSetReturnValues()
        {
            return ReturnValues != null;
        }

        /// <summary>
        /// Gets and sets the property ReturnValuesOnConditionCheckFailure. 
        /// <para>
        /// An optional parameter that returns the item attributes for a <code>PutItem</code>
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

        // Check to see if ReturnValuesOnConditionCheckFailure property is set
        internal bool IsSetReturnValuesOnConditionCheckFailure()
        {
            return ReturnValuesOnConditionCheckFailure != null;
        }

        /// <summary>
        /// Gets and sets the property TableName. 
        /// <para>
        /// The name of the table to contain the item.
        /// </para>
        /// </summary>
        public string? TableName { get; set; }

        // Check to see if TableName property is set
        internal bool IsSetTableName()
        {
            return TableName != null;
        }
    }
}