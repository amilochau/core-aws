﻿using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Container for the parameters to the UpdateItem operation.
    /// Edits an existing item's attributes, or adds a new item to the table if it does not
    /// already exist. You can put, delete, or add attribute values. You can also perform
    /// a conditional update on an existing item (insert a new attribute name-value pair if
    /// it doesn't exist, or replace an existing name-value pair if it has certain expected
    /// attribute values).
    /// 
    ///  
    /// <para>
    /// You can also return the item's attribute values in the same <code>UpdateItem</code>
    /// operation using the <code>ReturnValues</code> parameter.
    /// </para>
    /// </summary>
    public class UpdateItemRequest(Guid? userId) : AmazonDynamoDBRequest(userId)
    {
        /// <summary>
        /// Gets and sets the property AttributeUpdates. 
        /// <para>
        /// This is a legacy parameter. Use <code>UpdateExpression</code> instead. For more information,
        /// see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LegacyConditionalParameters.AttributeUpdates.html">AttributeUpdates</a>
        /// in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValueUpdate>? AttributeUpdates { get; set; }

        /// <summary>
        /// Gets and sets the property ConditionalOperator. 
        /// <para>
        /// This is a legacy parameter. Use <code>ConditionExpression</code> instead. For more
        /// information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LegacyConditionalParameters.ConditionalOperator.html">ConditionalOperator</a>
        /// in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public ConditionalOperator? ConditionalOperator { get; set; }

        /// <summary>
        /// Gets and sets the property ConditionExpression. 
        /// <para>
        /// A condition that must be satisfied in order for a conditional update to succeed.
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
        /// For more information about condition expressions, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Specifying
        /// Conditions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? ConditionExpression { get; set; }

        /// <summary>
        /// Gets and sets the property Expected. 
        /// <para>
        /// This is a legacy parameter. Use <code>ConditionExpression</code> instead. For more
        /// information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/LegacyConditionalParameters.Expected.html">Expected</a>
        /// in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, ExpectedAttributeValue>? Expected { get; set; }

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
        /// Words</a> in the <i>Amazon DynamoDB Developer Guide</i>.) To work around this, you
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
        /// For more information about expression attribute names, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.AccessingItemAttributes.html">Specifying
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
        /// For example, suppose that you wanted to check whether the value of the <code>ProductStatus</code>
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
        public Dictionary<string, AttributeValue>? ExpressionAttributeValues { get; set; }

        /// <summary>
        /// Gets and sets the property Key. 
        /// <para>
        /// The primary key of the item to be updated. Each element consists of an attribute name
        /// and a value for that attribute.
        /// </para>
        ///  
        /// <para>
        /// For the primary key, you must provide all of the attributes. For example, with a simple
        /// primary key, you only need to provide a value for the partition key. For a composite
        /// primary key, you must provide values for both the partition key and the sort key.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Key { get; set; }

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

        /// <summary>
        /// Gets and sets the property ReturnValues. 
        /// <para>
        /// Use <code>ReturnValues</code> if you want to get the item attributes as they appear
        /// before or after they are successfully updated. For <code>UpdateItem</code>, the valid
        /// values are:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>NONE</code> - If <code>ReturnValues</code> is not specified, or if its value
        /// is <code>NONE</code>, then nothing is returned. (This setting is the default for <code>ReturnValues</code>.)
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>ALL_OLD</code> - Returns all of the attributes of the item, as they appeared
        /// before the UpdateItem operation.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>UPDATED_OLD</code> - Returns only the updated attributes, as they appeared
        /// before the UpdateItem operation.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>ALL_NEW</code> - Returns all of the attributes of the item, as they appear
        /// after the UpdateItem operation.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>UPDATED_NEW</code> - Returns only the updated attributes, as they appear after
        /// the UpdateItem operation.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// There is no additional cost associated with requesting a return value aside from the
        /// small network and processing overhead of receiving a larger response. No read capacity
        /// units are consumed.
        /// </para>
        ///  
        /// <para>
        /// The values returned are strongly consistent.
        /// </para>
        /// </summary>
        public ReturnValue? ReturnValues { get; set; }

        /// <summary>
        /// Gets and sets the property ReturnValuesOnConditionCheckFailure. 
        /// <para>
        /// An optional parameter that returns the item attributes for an <code>UpdateItem</code>
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
        /// The name of the table containing the item to update.
        /// </para>
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// Gets and sets the property UpdateExpression. 
        /// <para>
        /// An expression that defines one or more attributes to be updated, the action to be
        /// performed on them, and new values for them.
        /// </para>
        ///  
        /// <para>
        /// The following action values are available for <code>UpdateExpression</code>.
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>SET</code> - Adds one or more attributes and values to an item. If any of these
        /// attributes already exist, they are replaced by the new values. You can also use <code>SET</code>
        /// to add or subtract from an attribute that is of type Number. For example: <code>SET
        /// myNum = myNum + :val</code> 
        /// </para>
        ///  
        /// <para>
        ///  <code>SET</code> supports the following functions:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>if_not_exists (path, operand)</code> - if the item does not contain an attribute
        /// at the specified path, then <code>if_not_exists</code> evaluates to operand; otherwise,
        /// it evaluates to path. You can use this function to avoid overwriting an attribute
        /// that may already be present in the item.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>list_append (operand, operand)</code> - evaluates to a list with a new element
        /// added to it. You can append the new element to the start or the end of the list by
        /// reversing the order of the operands.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// These function names are case-sensitive.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>REMOVE</code> - Removes one or more attributes from an item.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>ADD</code> - Adds the specified value to the item, if the attribute does not
        /// already exist. If the attribute does exist, then the behavior of <code>ADD</code>
        /// depends on the data type of the attribute:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// If the existing attribute is a number, and if <code>Value</code> is also a number,
        /// then <code>Value</code> is mathematically added to the existing attribute. If <code>Value</code>
        /// is a negative number, then it is subtracted from the existing attribute.
        /// </para>
        ///  <note> 
        /// <para>
        /// If you use <code>ADD</code> to increment or decrement a number value for an item that
        /// doesn't exist before the update, DynamoDB uses <code>0</code> as the initial value.
        /// </para>
        ///  
        /// <para>
        /// Similarly, if you use <code>ADD</code> for an existing item to increment or decrement
        /// an attribute value that doesn't exist before the update, DynamoDB uses <code>0</code>
        /// as the initial value. For example, suppose that the item you want to update doesn't
        /// have an attribute named <code>itemcount</code>, but you decide to <code>ADD</code>
        /// the number <code>3</code> to this attribute anyway. DynamoDB will create the <code>itemcount</code>
        /// attribute, set its initial value to <code>0</code>, and finally add <code>3</code>
        /// to it. The result will be a new <code>itemcount</code> attribute in the item, with
        /// a value of <code>3</code>.
        /// </para>
        ///  </note> </li> <li> 
        /// <para>
        /// If the existing data type is a set and if <code>Value</code> is also a set, then <code>Value</code>
        /// is added to the existing set. For example, if the attribute value is the set <code>[1,2]</code>,
        /// and the <code>ADD</code> action specified <code>[3]</code>, then the final attribute
        /// value is <code>[1,2,3]</code>. An error occurs if an <code>ADD</code> action is specified
        /// for a set attribute and the attribute type specified does not match the existing set
        /// type. 
        /// </para>
        ///  
        /// <para>
        /// Both sets must have the same primitive data type. For example, if the existing data
        /// type is a set of strings, the <code>Value</code> must also be a set of strings.
        /// </para>
        ///  </li> </ul> <important> 
        /// <para>
        /// The <code>ADD</code> action only supports Number and set data types. In addition,
        /// <code>ADD</code> can only be used on top-level attributes, not nested attributes.
        /// </para>
        ///  </important> </li> <li> 
        /// <para>
        ///  <code>DELETE</code> - Deletes an element from a set.
        /// </para>
        ///  
        /// <para>
        /// If a set of values is specified, then those values are subtracted from the old set.
        /// For example, if the attribute value was the set <code>[a,b,c]</code> and the <code>DELETE</code>
        /// action specifies <code>[a,c]</code>, then the final attribute value is <code>[b]</code>.
        /// Specifying an empty set is an error.
        /// </para>
        ///  <important> 
        /// <para>
        /// The <code>DELETE</code> action only supports set data types. In addition, <code>DELETE</code>
        /// can only be used on top-level attributes, not nested attributes.
        /// </para>
        ///  </important> </li> </ul> 
        /// <para>
        /// You can have many actions in a single expression, such as the following: <code>SET
        /// a=:value1, b=:value2 DELETE :value3, :value4, :value5</code> 
        /// </para>
        ///  
        /// <para>
        /// For more information on update expressions, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.Modifying.html">Modifying
        /// Items and Attributes</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? UpdateExpression { get; set; }

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