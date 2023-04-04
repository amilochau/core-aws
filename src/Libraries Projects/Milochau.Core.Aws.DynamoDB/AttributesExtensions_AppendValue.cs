using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>Extension methods for attribute value dictionary</summary>
    public static partial class AttributesExtensions
    {
        /// <summary>Append a string value</summary>
        /// <remarks>Trims the value</remarks>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return attributes;
            }
            return attributes.Append(new($":v_{key}", new AttributeValue { S = value.Trim() }));
        }

        /// <summary>Append an enumerable value of strings</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, IEnumerable<string?>? value)
        {
            if (value == null)
            {
                return attributes;
            }
            var formattedList = value.Select(x => x?.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (!formattedList.Any())
            {
                return attributes;
            }
            return attributes.Append(new($":v_{key}", new AttributeValue { SS = formattedList }));
        }

        /// <summary>Append a boolean value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, bool? value)
        {
            if (!value.HasValue || !value.Value)
            {
                return attributes;
            }
            return attributes.Append(new($":v_{key}", new AttributeValue { BOOL = value.Value }));
        }

        /// <summary>Append a double value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, double? value)
        {
            if (!value.HasValue)
            {
                return attributes;
            }
            return attributes.Append(new($":v_{key}", new AttributeValue { N = $"{value.Value}" }));
        }

        /// <summary>Append an enumerable value of doubles</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, IEnumerable<double?>? value)
        {
            if (value == null)
            {
                return attributes;
            }
            var formattedList = value.Where(x => x != null).Select(x => $"{x}").ToList();
            if (!formattedList.Any())
            {
                return attributes;
            }
            return attributes.Append(new($":v_{key}", new AttributeValue { NS = formattedList }));
        }

        /// <summary>Append a long value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, long value)
        {
            return attributes.Append(new($":v_{key}", new AttributeValue { N = $"{value}" }));
        }

        /// <summary>Append a long value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, decimal value)
        {
            return attributes.Append(new($":v_{key}", new AttributeValue { N = $"{value}" }));
        }

        /// <summary>Append an enum value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue<TEnum>(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, TEnum value)
            where TEnum : Enum
        {
            var intValue = Convert.ToInt32(value);
            return attributes.Append(new($":v_{key}", new AttributeValue { N = $"{intValue}" }));
        }

        /// <summary>Append an enum value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue<TEnum>(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, TEnum? value)
            where TEnum : struct, Enum
        {
            if (value == null)
            {
                return attributes;
            }
            var intValue = Convert.ToInt32(value);
            return attributes.Append(new($":v_{key}", new AttributeValue { N = $"{intValue}" }));
        }

        /// <summary>Append a date time offset value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, DateTimeOffset value)
        {
            return attributes.Append(new($":v_{key}", new AttributeValue { N = $"{value.ToUnixTimeSeconds()}" }));
        }

        /// <summary>Append a date time offset value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, DateTimeOffset? value)
        {
            if (value == null)
            {
                return attributes;
            }
            return attributes.Append(new($":v_{key}", new AttributeValue { N = $"{value.Value.ToUnixTimeSeconds()}" }));
        }

        /// <summary>Append an object value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, Dictionary<string, AttributeValue>? value)
        {
            if (value == null || !value.Any())
            {
                return attributes;
            }
            return attributes.Append(new($":v_{key}", new AttributeValue { M = value }));
        }

        /// <summary>Append an enumerable value of objects</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> AppendValue(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, IEnumerable<Dictionary<string, AttributeValue>>? value)
        {
            if (value == null || !value.Any())
            {
                return attributes;
            }
            return attributes.Append(new($":v_{key}", new AttributeValue { L = value.Select(x => new AttributeValue { M = x }).ToList() }));
        }
    }
}
