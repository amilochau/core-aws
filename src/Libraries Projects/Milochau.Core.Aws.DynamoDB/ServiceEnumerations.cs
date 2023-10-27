namespace Milochau.Core.Aws.DynamoDB
{
    // @todo To be improved with .NET 8 - see https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation-modes?pivots=dotnet-8-0#serialize-enum-fields-as-strings

    /// <summary>
    /// Constants used for properties of type StreamViewType.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<StreamViewType>))]
    public static class StreamViewType
    {
        /// <summary>
        /// Constant KEYS_ONLY for StreamViewType
        /// </summary>
        public const string KEYS_ONLY = "KEYS_ONLY";
        /// <summary>
        /// Constant NEW_AND_OLD_IMAGES for StreamViewType
        /// </summary>
        public const string NEW_AND_OLD_IMAGES = "NEW_AND_OLD_IMAGES";
        /// <summary>
        /// Constant NEW_IMAGE for StreamViewType
        /// </summary>
        public const string NEW_IMAGE = "NEW_IMAGE";
        /// <summary>
        /// Constant OLD_IMAGE for StreamViewType
        /// </summary>
        public const string OLD_IMAGE = "OLD_IMAGE";
    }

    /// <summary>
    /// Constants used for properties of type OperationType.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<OperationType>))]
    public static class OperationType
    {
        /// <summary>
        /// Constant INSERT for OperationType
        /// </summary>
        public const string INSERT = "INSERT";
        /// <summary>
        /// Constant MODIFY for OperationType
        /// </summary>
        public const string MODIFY = "MODIFY";
        /// <summary>
        /// Constant REMOVE for OperationType
        /// </summary>
        public const string REMOVE = "REMOVE";
    }

    /// <summary>
    /// Constants used for properties of type AttributeAction.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ReturnConsumedCapacity>))]
    public static class ReturnConsumedCapacity
    {
        /// <summary>
        /// Constant INDEXES for ReturnConsumedCapacity
        /// </summary>
        public const string INDEXES = "INDEXES";
        /// <summary>
        /// Constant NONE for ReturnConsumedCapacity
        /// </summary>
        public const string NONE = "NONE";
        /// <summary>
        /// Constant TOTAL for ReturnConsumedCapacity
        /// </summary>
        public const string TOTAL = "TOTAL";
    }

    /// <summary>
    /// Constants used for properties of type AttributeAction.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<AttributeAction>))]
    public static class AttributeAction
    {
        /// <summary>
        /// Constant ADD for AttributeAction
        /// </summary>
        public const string ADD = "ADD";
        /// <summary>
        /// Constant DELETE for AttributeAction
        /// </summary>
        public const string DELETE = "DELETE";
        /// <summary>
        /// Constant PUT for AttributeAction
        /// </summary>
        public const string PUT = "PUT";
    }

    /// <summary>
    /// Constants used for properties of type ReturnItemCollectionMetrics.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ReturnItemCollectionMetrics>))]
    public static class ReturnItemCollectionMetrics
    {
        /// <summary>
        /// Constant NONE for ReturnItemCollectionMetrics
        /// </summary>
        public const string NONE = "NONE";
        /// <summary>
        /// Constant SIZE for ReturnItemCollectionMetrics
        /// </summary>
        public const string SIZE = "SIZE";
    }

    /// <summary>
    /// Constants used for properties of type ConditionalOperator.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ConditionalOperator>))]
    public static class ConditionalOperator
    {
        /// <summary>
        /// Constant AND for ConditionalOperator
        /// </summary>
        public const string AND = "AND";
        /// <summary>
        /// Constant OR for ConditionalOperator
        /// </summary>
        public const string OR = "OR";
    }

    /// <summary>
    /// Constants used for properties of type ReturnValue.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ReturnValue>))]
    public static class ReturnValue
    {
        /// <summary>
        /// Constant ALL_NEW for ReturnValue
        /// </summary>
        public const string ALL_NEW = "ALL_NEW";
        /// <summary>
        /// Constant ALL_OLD for ReturnValue
        /// </summary>
        public const string ALL_OLD = "ALL_OLD";
        /// <summary>
        /// Constant NONE for ReturnValue
        /// </summary>
        public const string NONE = "NONE";
        /// <summary>
        /// Constant UPDATED_NEW for ReturnValue
        /// </summary>
        public const string UPDATED_NEW = "UPDATED_NEW";
        /// <summary>
        /// Constant UPDATED_OLD for ReturnValue
        /// </summary>
        public const string UPDATED_OLD = "UPDATED_OLD";
    }

    /// <summary>
    /// Constants used for properties of type ReturnValuesOnConditionCheckFailure.
    /// </summary>>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ReturnValuesOnConditionCheckFailure>))]
    public static class ReturnValuesOnConditionCheckFailure
    {
        /// <summary>
        /// Constant ALL_OLD for ReturnValuesOnConditionCheckFailure
        /// </summary>
        public const string ALL_OLD = "ALL_OLD";
        /// <summary>
        /// Constant NONE for ReturnValuesOnConditionCheckFailure
        /// </summary>
        public const string NONE = "NONE";
    }

    /// <summary>
    /// Constants used for properties of type Select.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<Select>))]
    public static class Select
    {
        /// <summary>
        /// Constant ALL_ATTRIBUTES for Select
        /// </summary>
        public const string ALL_ATTRIBUTES = "ALL_ATTRIBUTES";
        /// <summary>
        /// Constant ALL_PROJECTED_ATTRIBUTES for Select
        /// </summary>
        public const string ALL_PROJECTED_ATTRIBUTES = "ALL_PROJECTED_ATTRIBUTES";
        /// <summary>
        /// Constant COUNT for Select
        /// </summary>
        public const string COUNT = "COUNT";
        /// <summary>
        /// Constant SPECIFIC_ATTRIBUTES for Select
        /// </summary>
        public const string SPECIFIC_ATTRIBUTES = "SPECIFIC_ATTRIBUTES";
    }

    /// <summary>
    /// Constants used for properties of type ComparisonOperator.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ComparisonOperator>))]
    public static class ComparisonOperator
    {
        /// <summary>
        /// Constant BEGINS_WITH for ComparisonOperator
        /// </summary>
        public const string BEGINS_WITH = "BEGINS_WITH";
        /// <summary>
        /// Constant BETWEEN for ComparisonOperator
        /// </summary>
        public const string BETWEEN = "BETWEEN";
        /// <summary>
        /// Constant CONTAINS for ComparisonOperator
        /// </summary>
        public const string CONTAINS = "CONTAINS";
        /// <summary>
        /// Constant EQ for ComparisonOperator
        /// </summary>
        public const string EQ = "EQ";
        /// <summary>
        /// Constant GE for ComparisonOperator
        /// </summary>
        public const string GE = "GE";
        /// <summary>
        /// Constant GT for ComparisonOperator
        /// </summary>
        public const string GT = "GT";
        /// <summary>
        /// Constant IN for ComparisonOperator
        /// </summary>
        public const string IN = "IN";
        /// <summary>
        /// Constant LE for ComparisonOperator
        /// </summary>
        public const string LE = "LE";
        /// <summary>
        /// Constant LT for ComparisonOperator
        /// </summary>
        public const string LT = "LT";
        /// <summary>
        /// Constant NE for ComparisonOperator
        /// </summary>
        public const string NE = "NE";
        /// <summary>
        /// Constant NOT_CONTAINS for ComparisonOperator
        /// </summary>
        public const string NOT_CONTAINS = "NOT_CONTAINS";
        /// <summary>
        /// Constant NOT_NULL for ComparisonOperator
        /// </summary>
        public const string NOT_NULL = "NOT_NULL";
        /// <summary>
        /// Constant NULL for ComparisonOperator
        /// </summary>
        public const string NULL = "NULL";
    }
}
