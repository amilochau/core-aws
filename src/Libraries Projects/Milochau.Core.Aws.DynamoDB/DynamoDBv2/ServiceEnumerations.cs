﻿using Amazon.Runtime;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/ServiceEnumerations.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2
{
    /// <summary>
    /// Constants used for properties of type AttributeAction.
    /// </summary>
    public class AttributeAction : ConstantClass
    {

        /// <summary>
        /// Constant ADD for AttributeAction
        /// </summary>
        public static readonly AttributeAction ADD = new("ADD");
        /// <summary>
        /// Constant DELETE for AttributeAction
        /// </summary>
        public static readonly AttributeAction DELETE = new("DELETE");
        /// <summary>
        /// Constant PUT for AttributeAction
        /// </summary>
        public static readonly AttributeAction PUT = new("PUT");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public AttributeAction(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Finds the constant for the unique value.
        /// </summary>
        /// <param name="value">The unique value for the constant</param>
        /// <returns>The constant for the unique value</returns>
        public static AttributeAction FindValue(string value)
        {
            return FindValue<AttributeAction>(value);
        }

        /// <summary>
        /// Utility method to convert strings to the constant class.
        /// </summary>
        /// <param name="value">The string value to convert to the constant class.</param>
        /// <returns></returns>
        public static implicit operator AttributeAction(string value)
        {
            return FindValue(value);
        }
    }

    /// <summary>
    /// Constants used for properties of type ReturnConsumedCapacity.
    /// </summary>
    public class ReturnConsumedCapacity : ConstantClass
    {
        /// <summary>
        /// Constant INDEXES for ReturnConsumedCapacity
        /// </summary>
        public static readonly ReturnConsumedCapacity INDEXES = new("INDEXES");
        /// <summary>
        /// Constant NONE for ReturnConsumedCapacity
        /// </summary>
        public static readonly ReturnConsumedCapacity NONE = new("NONE");
        /// <summary>
        /// Constant TOTAL for ReturnConsumedCapacity
        /// </summary>
        public static readonly ReturnConsumedCapacity TOTAL = new("TOTAL");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public ReturnConsumedCapacity(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Finds the constant for the unique value.
        /// </summary>
        /// <param name="value">The unique value for the constant</param>
        /// <returns>The constant for the unique value</returns>
        public static ReturnConsumedCapacity FindValue(string value)
        {
            return FindValue<ReturnConsumedCapacity>(value);
        }

        /// <summary>
        /// Utility method to convert strings to the constant class.
        /// </summary>
        /// <param name="value">The string value to convert to the constant class.</param>
        /// <returns></returns>
        public static implicit operator ReturnConsumedCapacity(string value)
        {
            return FindValue(value);
        }
    }

    /// <summary>
    /// Constants used for properties of type ReturnItemCollectionMetrics.
    /// </summary>
    public class ReturnItemCollectionMetrics : ConstantClass
    {

        /// <summary>
        /// Constant NONE for ReturnItemCollectionMetrics
        /// </summary>
        public static readonly ReturnItemCollectionMetrics NONE = new("NONE");
        /// <summary>
        /// Constant SIZE for ReturnItemCollectionMetrics
        /// </summary>
        public static readonly ReturnItemCollectionMetrics SIZE = new("SIZE");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public ReturnItemCollectionMetrics(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Finds the constant for the unique value.
        /// </summary>
        /// <param name="value">The unique value for the constant</param>
        /// <returns>The constant for the unique value</returns>
        public static ReturnItemCollectionMetrics FindValue(string value)
        {
            return FindValue<ReturnItemCollectionMetrics>(value);
        }

        /// <summary>
        /// Utility method to convert strings to the constant class.
        /// </summary>
        /// <param name="value">The string value to convert to the constant class.</param>
        /// <returns></returns>
        public static implicit operator ReturnItemCollectionMetrics(string value)
        {
            return FindValue(value);
        }
    }

    /// <summary>
    /// Constants used for properties of type ConditionalOperator.
    /// </summary>
    public class ConditionalOperator : ConstantClass
    {

        /// <summary>
        /// Constant AND for ConditionalOperator
        /// </summary>
        public static readonly ConditionalOperator AND = new("AND");
        /// <summary>
        /// Constant OR for ConditionalOperator
        /// </summary>
        public static readonly ConditionalOperator OR = new("OR");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public ConditionalOperator(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Finds the constant for the unique value.
        /// </summary>
        /// <param name="value">The unique value for the constant</param>
        /// <returns>The constant for the unique value</returns>
        public static ConditionalOperator FindValue(string value)
        {
            return FindValue<ConditionalOperator>(value);
        }

        /// <summary>
        /// Utility method to convert strings to the constant class.
        /// </summary>
        /// <param name="value">The string value to convert to the constant class.</param>
        /// <returns></returns>
        public static implicit operator ConditionalOperator(string value)
        {
            return FindValue(value);
        }
    }

    /// <summary>
    /// Constants used for properties of type ReturnValue.
    /// </summary>
    public class ReturnValue : ConstantClass
    {

        /// <summary>
        /// Constant ALL_NEW for ReturnValue
        /// </summary>
        public static readonly ReturnValue ALL_NEW = new("ALL_NEW");
        /// <summary>
        /// Constant ALL_OLD for ReturnValue
        /// </summary>
        public static readonly ReturnValue ALL_OLD = new("ALL_OLD");
        /// <summary>
        /// Constant NONE for ReturnValue
        /// </summary>
        public static readonly ReturnValue NONE = new("NONE");
        /// <summary>
        /// Constant UPDATED_NEW for ReturnValue
        /// </summary>
        public static readonly ReturnValue UPDATED_NEW = new("UPDATED_NEW");
        /// <summary>
        /// Constant UPDATED_OLD for ReturnValue
        /// </summary>
        public static readonly ReturnValue UPDATED_OLD = new("UPDATED_OLD");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public ReturnValue(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Finds the constant for the unique value.
        /// </summary>
        /// <param name="value">The unique value for the constant</param>
        /// <returns>The constant for the unique value</returns>
        public static ReturnValue FindValue(string value)
        {
            return FindValue<ReturnValue>(value);
        }

        /// <summary>
        /// Utility method to convert strings to the constant class.
        /// </summary>
        /// <param name="value">The string value to convert to the constant class.</param>
        /// <returns></returns>
        public static implicit operator ReturnValue(string value)
        {
            return FindValue(value);
        }
    }

    /// <summary>
    /// Constants used for properties of type ReturnValuesOnConditionCheckFailure.
    /// </summary>
    public class ReturnValuesOnConditionCheckFailure : ConstantClass
    {

        /// <summary>
        /// Constant ALL_OLD for ReturnValuesOnConditionCheckFailure
        /// </summary>
        public static readonly ReturnValuesOnConditionCheckFailure ALL_OLD = new("ALL_OLD");
        /// <summary>
        /// Constant NONE for ReturnValuesOnConditionCheckFailure
        /// </summary>
        public static readonly ReturnValuesOnConditionCheckFailure NONE = new("NONE");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public ReturnValuesOnConditionCheckFailure(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Finds the constant for the unique value.
        /// </summary>
        /// <param name="value">The unique value for the constant</param>
        /// <returns>The constant for the unique value</returns>
        public static ReturnValuesOnConditionCheckFailure FindValue(string value)
        {
            return FindValue<ReturnValuesOnConditionCheckFailure>(value);
        }

        /// <summary>
        /// Utility method to convert strings to the constant class.
        /// </summary>
        /// <param name="value">The string value to convert to the constant class.</param>
        /// <returns></returns>
        public static implicit operator ReturnValuesOnConditionCheckFailure(string value)
        {
            return FindValue(value);
        }
    }

    /// <summary>
    /// Constants used for properties of type Select.
    /// </summary>
    public class Select : ConstantClass
    {

        /// <summary>
        /// Constant ALL_ATTRIBUTES for Select
        /// </summary>
        public static readonly Select ALL_ATTRIBUTES = new("ALL_ATTRIBUTES");
        /// <summary>
        /// Constant ALL_PROJECTED_ATTRIBUTES for Select
        /// </summary>
        public static readonly Select ALL_PROJECTED_ATTRIBUTES = new("ALL_PROJECTED_ATTRIBUTES");
        /// <summary>
        /// Constant COUNT for Select
        /// </summary>
        public static readonly Select COUNT = new("COUNT");
        /// <summary>
        /// Constant SPECIFIC_ATTRIBUTES for Select
        /// </summary>
        public static readonly Select SPECIFIC_ATTRIBUTES = new("SPECIFIC_ATTRIBUTES");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public Select(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Finds the constant for the unique value.
        /// </summary>
        /// <param name="value">The unique value for the constant</param>
        /// <returns>The constant for the unique value</returns>
        public static Select FindValue(string value)
        {
            return FindValue<Select>(value);
        }

        /// <summary>
        /// Utility method to convert strings to the constant class.
        /// </summary>
        /// <param name="value">The string value to convert to the constant class.</param>
        /// <returns></returns>
        public static implicit operator Select(string value)
        {
            return FindValue(value);
        }
    }

    /// <summary>
    /// Constants used for properties of type ComparisonOperator.
    /// </summary>
    public class ComparisonOperator : ConstantClass
    {

        /// <summary>
        /// Constant BEGINS_WITH for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator BEGINS_WITH = new("BEGINS_WITH");
        /// <summary>
        /// Constant BETWEEN for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator BETWEEN = new("BETWEEN");
        /// <summary>
        /// Constant CONTAINS for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator CONTAINS = new("CONTAINS");
        /// <summary>
        /// Constant EQ for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator EQ = new("EQ");
        /// <summary>
        /// Constant GE for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator GE = new("GE");
        /// <summary>
        /// Constant GT for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator GT = new("GT");
        /// <summary>
        /// Constant IN for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator IN = new("IN");
        /// <summary>
        /// Constant LE for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator LE = new("LE");
        /// <summary>
        /// Constant LT for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator LT = new("LT");
        /// <summary>
        /// Constant NE for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator NE = new("NE");
        /// <summary>
        /// Constant NOT_CONTAINS for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator NOT_CONTAINS = new("NOT_CONTAINS");
        /// <summary>
        /// Constant NOT_NULL for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator NOT_NULL = new("NOT_NULL");
        /// <summary>
        /// Constant NULL for ComparisonOperator
        /// </summary>
        public static readonly ComparisonOperator NULL = new("NULL");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public ComparisonOperator(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Finds the constant for the unique value.
        /// </summary>
        /// <param name="value">The unique value for the constant</param>
        /// <returns>The constant for the unique value</returns>
        public static ComparisonOperator FindValue(string value)
        {
            return FindValue<ComparisonOperator>(value);
        }

        /// <summary>
        /// Utility method to convert strings to the constant class.
        /// </summary>
        /// <param name="value">The string value to convert to the constant class.</param>
        /// <returns></returns>
        public static implicit operator ComparisonOperator(string value)
        {
            return FindValue(value);
        }
    }
}