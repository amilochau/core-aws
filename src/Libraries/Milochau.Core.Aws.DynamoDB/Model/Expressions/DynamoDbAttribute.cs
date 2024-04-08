using Milochau.Core.Aws.DynamoDB.Helpers;
using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model.Expressions
{
    /// <summary>Attribute for DynamoDB</summary>
    // @todo [Obsolete]
    public class DynamoDbAttribute(string key, AttributeValue value)
    {
        /// <summary>Implicit convertor</summary>
        public static implicit operator DynamoDbAttribute(KeyValuePair<string, AttributeValue> pair) => new(pair.Key, pair.Value);

        /// <summary>Implicit convertor</summary>
        public static implicit operator KeyValuePair<string, AttributeValue>(DynamoDbAttribute attribute) => new KeyValuePair<string, AttributeValue>(attribute.Key, attribute.Value);

        /// <summary>Attribute key</summary>
        public string Key { get; } = key;

        /// <summary>Attribute value</summary>
        public AttributeValue Value { get; } = value;

        /// <summary>Convert as a <see cref="KeyValuePair{TKey, TValue}"/></summary>
        public KeyValuePair<string, AttributeValue> AsKeyValuePair() => new(Key, Value);

        /// <summary>Deconstructor</summary>
        public void Deconstruct(out string key, out AttributeValue value)
        {
            key = Key;
            value = Value;
        }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, string? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, Guid? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, IEnumerable<string>? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, bool? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, double? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, long? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, decimal? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, IEnumerable<double?>? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, Enum? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, DateTimeOffset? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, IDynamoDbFormattableEntity? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, Dictionary<string, AttributeValue>? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, IEnumerable<DynamoDbAttribute>? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, IEnumerable<IDynamoDbFormattableEntity>? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, IEnumerable<Dictionary<string, AttributeValue>>? value) : this(key, new AttributeValue(value)) { }

        /// <summary>Constructor</summary>
        public DynamoDbAttribute(string key, IEnumerable<IEnumerable<DynamoDbAttribute>>? value) : this(key, new AttributeValue(value)) { }
    }
}
