using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>Extension methods for attribute value dictionary</summary>
    public static class AttributeValueDictionaryExtensions
    {
        /// <summary>Append a string value</summary>
        /// <remarks>Trims the value</remarks>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return attributes;
            }
            return attributes.Append(new(key, new AttributeValue { S = value.Trim() }));
        }

        /// <summary>Append an enumerable value of strings</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, IEnumerable<string?>? value)
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
            return attributes.Append(new(key, new AttributeValue { SS = formattedList }));
        }

        /// <summary>Append a boolean value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, bool? value)
        {
            if (!value.HasValue || !value.Value)
            {
                return attributes;
            }
            return attributes.Append(new(key, new AttributeValue { BOOL = value.Value }));
        }

        /// <summary>Append a double value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, double? value)
        {
            if (!value.HasValue)
            {
                return attributes;
            }
            return attributes.Append(new(key, new AttributeValue { N = $"{value.Value}" }));
        }

        /// <summary>Append an enumerable value of doubles</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, IEnumerable<double?>? value)
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
            return attributes.Append(new(key, new AttributeValue { NS = formattedList }));
        }

        /// <summary>Append a long value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, long value)
        {
            return attributes.Append(new(key, new AttributeValue { N = $"{value}" }));
        }

        /// <summary>Append a long value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, decimal value)
        {
            return attributes.Append(new(key, new AttributeValue { N = $"{value}" }));
        }

        /// <summary>Append an enum value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append<TEnum>(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, TEnum value)
            where TEnum : Enum
        {
            var intValue = Convert.ToInt32(value);
            return attributes.Append(new(key, new AttributeValue { N = $"{intValue}" }));
        }

        /// <summary>Append an enum value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append<TEnum>(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, TEnum? value)
            where TEnum : struct, Enum
        {
            if (value == null)
            {
                return attributes;
            }
            var intValue = Convert.ToInt32(value);
            return attributes.Append(new(key, new AttributeValue { N = $"{intValue}" }));
        }

        /// <summary>Append a date time offset value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, DateTimeOffset value)
        {
            return attributes.Append(new(key, new AttributeValue { N = $"{value.ToUnixTimeSeconds()}" }));
        }

        /// <summary>Append an object value</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, Dictionary<string, AttributeValue>? value)
        {
            if (value == null || !value.Any())
            {
                return attributes;
            }
            return attributes.Append(new(key, new AttributeValue { M = value }));
        }

        /// <summary>Append an enumerable value of objects</summary>
        public static IEnumerable<KeyValuePair<string, AttributeValue>> Append(this IEnumerable<KeyValuePair<string, AttributeValue>> attributes, string key, IEnumerable<Dictionary<string, AttributeValue>>? value)
        {
            if (value == null || !value.Any())
            {
                return attributes;
            }
            return attributes.Append(new(key, new AttributeValue { L = value.Select(x => new AttributeValue { M = x }).ToList() }));
        }

        /// <summary>Read a value as an object</summary>
        public static Dictionary<string, AttributeValue?> ReadObject(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return attributes[key]!.M;
        }

        /// <summary>Read an optional value as an object</summary>
        public static Dictionary<string, AttributeValue?>? ReadObjectOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.M;
            }
            return null;
        }

        /// <summary>Read a value as a list of objects</summary>
        public static List<Dictionary<string, AttributeValue?>> ReadList(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return attributes[key]!.L.Select(x => x.M).ToList();
        }

        /// <summary>Read an optional value as a list of objects</summary>
        public static List<Dictionary<string, AttributeValue?>>? ReadListOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.L.Select(x => x.M).ToList();
            }
            return null;
        }

        /// <summary>Read a value as a list of string</summary>
        public static List<string> ReadListString(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return attributes[key]!.SS;
        }

        /// <summary>Read an optional value as a list of string</summary>
        public static List<string>? ReadListStringOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.SS;
            }
            return null;
        }

        /// <summary>Read a value as a string</summary>
        public static string ReadString(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return attributes[key]!.S;
        }

        /// <summary>Read an optional value as a string</summary>
        public static string? ReadStringOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.S;
            }
            return null;
        }

        /// <summary>Read a value as an integer</summary>
        public static int ReadInt(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return int.Parse(attributes[key]!.N);
        }

        /// <summary>Read an optional value as an integer</summary>
        public static int? ReadIntOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return int.Parse(attribute.N);
            }
            return null;
        }

        /// <summary>Read a value as an decimal</summary>
        public static decimal ReadDecimal(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return decimal.Parse(attributes[key]!.N);
        }

        /// <summary>Read an optional value as an decimal</summary>
        public static decimal? ReadDecimalOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return decimal.Parse(attribute.N);
            }
            return null;
        }

        /// <summary>Read a value as a list of doubles</summary>
        public static List<double> ReadListDouble(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return attributes[key]!.NS.Select(x => double.Parse(x)).ToList();
        }

        /// <summary>Read an optional value as a list of doubles</summary>
        public static List<double>? ReadListDoubleOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.NS.Select(x => double.Parse(x)).ToList();
            }
            return null;
        }

        /// <summary>Read a value as a double</summary>
        public static double ReadDouble(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return double.Parse(attributes[key]!.N);
        }

        /// <summary>Read an optional value as a double</summary>
        public static double? ReadDoubleOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return double.Parse(attribute.N);
            }
            return null;
        }

        /// <summary>Read an optional value as an enumeration</summary>
        public static TEnum? ReadEnumOptional<TEnum>(this Dictionary<string, AttributeValue?> attributes, string key)
            where TEnum : struct, Enum
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return Enum.Parse<TEnum>(attribute.N);
            }
            return null;
        }

        /// <summary>Read an optional value as a boolean</summary>
        public static bool? ReadBoolOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.BOOL;
            }
            return null;
        }

        /// <summary>Read a value as a date time offset</summary>
        public static DateTimeOffset ReadDateTimeOffset(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(attributes[key]!.N));
        }

        /// <summary>Read an optional value as a date time offset</summary>
        public static DateTimeOffset? ReadDateTimeOffsetOptional(this Dictionary<string, AttributeValue?> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return DateTimeOffset.FromUnixTimeSeconds(long.Parse(attribute.N));
            }
            return null;
        }
    }
}
