using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>
    /// Constants used for properties of type StreamViewType.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<StreamViewType>))]
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
    [JsonConverter(typeof(JsonStringEnumConverter<OperationType>))]
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
    /// Constants used for properties of type ReturnConsumedCapacity.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ReturnConsumedCapacity>))]
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
    /// Constants used for properties of type ReturnItemCollectionMetrics.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ReturnItemCollectionMetrics>))]
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
    /// Constants used for properties of type ReturnValue.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<ReturnValue>))]
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
    [JsonConverter(typeof(JsonStringEnumConverter<ReturnValuesOnConditionCheckFailure>))]
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
    [JsonConverter(typeof(JsonStringEnumConverter<Select>))]
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
}
