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
    /// You can also return the item's attribute values in the same <c>UpdateItem</c>
    /// operation using the <c>ReturnValues</c> parameter.
    /// </para>
    /// </summary>
    public class UpdateItemRequest(Guid? userId) : AmazonDynamoDBRequest(userId)
    {
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
        /// For more information about condition expressions, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Specifying
        /// Conditions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? ConditionExpression { get; set; }

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
        /// Words</a> in the <i>Amazon DynamoDB Developer Guide</i>.) To work around this, you
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
        /// For example, suppose that you wanted to check whether the value of the <c>ProductStatus</c>
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
        /// Use <c>ReturnValues</c> if you want to get the item attributes as they appear
        /// before or after they are successfully updated. For <c>UpdateItem</c>, the valid
        /// values are:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>NONE</c> - If <c>ReturnValues</c> is not specified, or if its value
        /// is <c>NONE</c>, then nothing is returned. (This setting is the default for <c>ReturnValues</c>.)
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>ALL_OLD</c> - Returns all of the attributes of the item, as they appeared
        /// before the UpdateItem operation.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>UPDATED_OLD</c> - Returns only the updated attributes, as they appeared
        /// before the UpdateItem operation.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>ALL_NEW</c> - Returns all of the attributes of the item, as they appear
        /// after the UpdateItem operation.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>UPDATED_NEW</c> - Returns only the updated attributes, as they appear after
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
        /// An optional parameter that returns the item attributes for an <c>UpdateItem</c>
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
        /// The following action values are available for <c>UpdateExpression</c>.
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>SET</c> - Adds one or more attributes and values to an item. If any of these
        /// attributes already exist, they are replaced by the new values. You can also use <c>SET</c>
        /// to add or subtract from an attribute that is of type Number. For example: <c>SET
        /// myNum = myNum + :val</c> 
        /// </para>
        ///  
        /// <para>
        ///  <c>SET</c> supports the following functions:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>if_not_exists (path, operand)</c> - if the item does not contain an attribute
        /// at the specified path, then <c>if_not_exists</c> evaluates to operand; otherwise,
        /// it evaluates to path. You can use this function to avoid overwriting an attribute
        /// that may already be present in the item.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>list_append (operand, operand)</c> - evaluates to a list with a new element
        /// added to it. You can append the new element to the start or the end of the list by
        /// reversing the order of the operands.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// These function names are case-sensitive.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>REMOVE</c> - Removes one or more attributes from an item.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>ADD</c> - Adds the specified value to the item, if the attribute does not
        /// already exist. If the attribute does exist, then the behavior of <c>ADD</c>
        /// depends on the data type of the attribute:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// If the existing attribute is a number, and if <c>Value</c> is also a number,
        /// then <c>Value</c> is mathematically added to the existing attribute. If <c>Value</c>
        /// is a negative number, then it is subtracted from the existing attribute.
        /// </para>
        ///  <note> 
        /// <para>
        /// If you use <c>ADD</c> to increment or decrement a number value for an item that
        /// doesn't exist before the update, DynamoDB uses <c>0</c> as the initial value.
        /// </para>
        ///  
        /// <para>
        /// Similarly, if you use <c>ADD</c> for an existing item to increment or decrement
        /// an attribute value that doesn't exist before the update, DynamoDB uses <c>0</c>
        /// as the initial value. For example, suppose that the item you want to update doesn't
        /// have an attribute named <c>itemcount</c>, but you decide to <c>ADD</c>
        /// the number <c>3</c> to this attribute anyway. DynamoDB will create the <c>itemcount</c>
        /// attribute, set its initial value to <c>0</c>, and finally add <c>3</c>
        /// to it. The result will be a new <c>itemcount</c> attribute in the item, with
        /// a value of <c>3</c>.
        /// </para>
        ///  </note> </li> <li> 
        /// <para>
        /// If the existing data type is a set and if <c>Value</c> is also a set, then <c>Value</c>
        /// is added to the existing set. For example, if the attribute value is the set <c>[1,2]</c>,
        /// and the <c>ADD</c> action specified <c>[3]</c>, then the final attribute
        /// value is <c>[1,2,3]</c>. An error occurs if an <c>ADD</c> action is specified
        /// for a set attribute and the attribute type specified does not match the existing set
        /// type. 
        /// </para>
        ///  
        /// <para>
        /// Both sets must have the same primitive data type. For example, if the existing data
        /// type is a set of strings, the <c>Value</c> must also be a set of strings.
        /// </para>
        ///  </li> </ul> <important> 
        /// <para>
        /// The <c>ADD</c> action only supports Number and set data types. In addition,
        /// <c>ADD</c> can only be used on top-level attributes, not nested attributes.
        /// </para>
        ///  </important> </li> <li> 
        /// <para>
        ///  <c>DELETE</c> - Deletes an element from a set.
        /// </para>
        ///  
        /// <para>
        /// If a set of values is specified, then those values are subtracted from the old set.
        /// For example, if the attribute value was the set <c>[a,b,c]</c> and the <c>DELETE</c>
        /// action specifies <c>[a,c]</c>, then the final attribute value is <c>[b]</c>.
        /// Specifying an empty set is an error.
        /// </para>
        ///  <important> 
        /// <para>
        /// The <c>DELETE</c> action only supports set data types. In addition, <c>DELETE</c>
        /// can only be used on top-level attributes, not nested attributes.
        /// </para>
        ///  </important> </li> </ul> 
        /// <para>
        /// You can have many actions in a single expression, such as the following: <c>SET
        /// a=:value1, b=:value2 DELETE :value3, :value4, :value5</c> 
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