namespace Milochau.Core.Aws.DynamoDB
{
    // @todo To be improved with .NET 8 - see https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation-modes?pivots=dotnet-8-0#serialize-enum-fields-as-strings

    /// <summary>
    /// Constants used for properties of type StreamViewType.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<StreamViewType>))]
    public enum StreamViewType
    {
        /// <summary>
        /// Constant KEYS_ONLY for StreamViewType
        /// </summary>
        KEYS_ONLY,
        /// <summary>
        /// Constant NEW_AND_OLD_IMAGES for StreamViewType
        /// </summary>
        NEW_AND_OLD_IMAGES,
        /// <summary>
        /// Constant NEW_IMAGE for StreamViewType
        /// </summary>
        NEW_IMAGE,
        /// <summary>
        /// Constant OLD_IMAGE for StreamViewType
        /// </summary>
        OLD_IMAGE,
    }

    /// <summary>
    /// Constants used for properties of type OperationType.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<OperationType>))]
    public enum OperationType
    {
        /// <summary>
        /// Constant INSERT for OperationType
        /// </summary>
        INSERT,
        /// <summary>
        /// Constant MODIFY for OperationType
        /// </summary>
        MODIFY,
        /// <summary>
        /// Constant REMOVE for OperationType
        /// </summary>
        REMOVE,
    }

    /// <summary>
    /// Constants used for properties of type AttributeAction.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ReturnConsumedCapacity>))]
    public enum ReturnConsumedCapacity
    {
        /// <summary>
        /// Constant INDEXES for ReturnConsumedCapacity
        /// </summary>
        INDEXES,
        /// <summary>
        /// Constant NONE for ReturnConsumedCapacity
        /// </summary>
        NONE,
        /// <summary>
        /// Constant TOTAL for ReturnConsumedCapacity
        /// </summary>
        TOTAL,
    }

    /// <summary>
    /// Constants used for properties of type AttributeAction.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<AttributeAction>))]
    public enum AttributeAction
    {
        /// <summary>
        /// Constant ADD for AttributeAction
        /// </summary>
        ADD,
        /// <summary>
        /// Constant DELETE for AttributeAction
        /// </summary>
        DELETE,
        /// <summary>
        /// Constant PUT for AttributeAction
        /// </summary>
        PUT,
    }

    /// <summary>
    /// Constants used for properties of type ReturnItemCollectionMetrics.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ReturnItemCollectionMetrics>))]
    public enum ReturnItemCollectionMetrics
    {
        /// <summary>
        /// Constant NONE for ReturnItemCollectionMetrics
        /// </summary>
        NONE,
        /// <summary>
        /// Constant SIZE for ReturnItemCollectionMetrics
        /// </summary>
        SIZE,
    }

    /// <summary>
    /// Constants used for properties of type ConditionalOperator.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ConditionalOperator>))]
    public enum ConditionalOperator
    {
        /// <summary>
        /// Constant AND for ConditionalOperator
        /// </summary>
        AND,
        /// <summary>
        /// Constant OR for ConditionalOperator
        /// </summary>
        OR,
    }

    /// <summary>
    /// Constants used for properties of type ReturnValue.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ReturnValue>))]
    public enum ReturnValue
    {
        /// <summary>
        /// Constant ALL_NEW for ReturnValue
        /// </summary>
        ALL_NEW,
        /// <summary>
        /// Constant ALL_OLD for ReturnValue
        /// </summary>
        ALL_OLD,
        /// <summary>
        /// Constant NONE for ReturnValue
        /// </summary>
        NONE,
        /// <summary>
        /// Constant UPDATED_NEW for ReturnValue
        /// </summary>
        UPDATED_NEW,
        /// <summary>
        /// Constant UPDATED_OLD for ReturnValue
        /// </summary>
        UPDATED_OLD,
    }

    /// <summary>
    /// Constants used for properties of type ReturnValuesOnConditionCheckFailure.
    /// </summary>>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ReturnValuesOnConditionCheckFailure>))]
    public enum ReturnValuesOnConditionCheckFailure
    {
        /// <summary>
        /// Constant ALL_OLD for ReturnValuesOnConditionCheckFailure
        /// </summary>
        ALL_OLD,
        /// <summary>
        /// Constant NONE for ReturnValuesOnConditionCheckFailure
        /// </summary>
        NONE,
    }

    /// <summary>
    /// Constants used for properties of type Select.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<Select>))]
    public enum Select
    {
        /// <summary>
        /// Constant ALL_ATTRIBUTES for Select
        /// </summary>
        ALL_ATTRIBUTES,
        /// <summary>
        /// Constant ALL_PROJECTED_ATTRIBUTES for Select
        /// </summary>
        ALL_PROJECTED_ATTRIBUTES,
        /// <summary>
        /// Constant COUNT for Select
        /// </summary>
        COUNT,
        /// <summary>
        /// Constant SPECIFIC_ATTRIBUTES for Select
        /// </summary>
        SPECIFIC_ATTRIBUTES,
    }

    /// <summary>
    /// Constants used for properties of type ComparisonOperator.
    /// </summary>
    // @todo [JsonConverter(typeof(JsonStringEnumConverter<ComparisonOperator>))]
    public enum ComparisonOperator
    {
        /// <summary>
        /// Constant BEGINS_WITH for ComparisonOperator
        /// </summary>
        BEGINS_WITH,
        /// <summary>
        /// Constant BETWEEN for ComparisonOperator
        /// </summary>
        BETWEEN,
        /// <summary>
        /// Constant CONTAINS for ComparisonOperator
        /// </summary>
        CONTAINS,
        /// <summary>
        /// Constant EQ for ComparisonOperator
        /// </summary>
        EQ,
        /// <summary>
        /// Constant GE for ComparisonOperator
        /// </summary>
        GE,
        /// <summary>
        /// Constant GT for ComparisonOperator
        /// </summary>
        GT,
        /// <summary>
        /// Constant IN for ComparisonOperator
        /// </summary>
        IN,
        /// <summary>
        /// Constant LE for ComparisonOperator
        /// </summary>
        LE,
        /// <summary>
        /// Constant LT for ComparisonOperator
        /// </summary>
        LT,
        /// <summary>
        /// Constant NE for ComparisonOperator
        /// </summary>
        NE,
        /// <summary>
        /// Constant NOT_CONTAINS for ComparisonOperator
        /// </summary>
        NOT_CONTAINS,
        /// <summary>
        /// Constant NOT_NULL for ComparisonOperator
        /// </summary>
        NOT_NULL,
        /// <summary>
        /// Constant NULL for ComparisonOperator
        /// </summary>
        NULL,
    }
}
