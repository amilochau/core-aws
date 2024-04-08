using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Extension methods for attribute value dictionary</summary>
    public static partial class AttributesExtensions
    {
        /// <summary>Read a value as an object</summary>
        [Obsolete]
        public static TEntity ReadObject<TEntity>(this Dictionary<string, AttributeValue> attributes, string key)
            where TEntity: IDynamoDbParsableEntity<TEntity>
        {
            return TEntity.ParseFromDynamoDb(ReadObject(attributes, key));
        }

        /// <summary>Read a value as an object</summary>
        [Obsolete]
        public static Dictionary<string, AttributeValue> ReadObject(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return attributes[key].M!;
        }

        /// <summary>Read an optional value as an object</summary>
        [Obsolete]
        public static TEntity? ReadObjectOptional<TEntity>(this Dictionary<string, AttributeValue> attributes, string key)
            where TEntity : class, IDynamoDbParsableEntity<TEntity>
        {
            var rawEntity = ReadObjectOptional(attributes, key);
            if (rawEntity == null)
            {
                return null;
            }
            return TEntity.ParseFromDynamoDb(rawEntity);
        }

        /// <summary>Read an optional value as an object</summary>
        [Obsolete]
        public static Dictionary<string, AttributeValue>? ReadObjectOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.M;
            }
            return null;
        }

        /// <summary>Read a value as a list of objects</summary>
        [Obsolete]
        public static List<TEntity> ReadList<TEntity>(this Dictionary<string, AttributeValue> attributes, string key)
            where TEntity: IDynamoDbParsableEntity<TEntity>
        {
            return attributes[key].L!.Select(x => TEntity.ParseFromDynamoDb(x.M!)).ToList();
        }

        /// <summary>Read a value as a list of objects</summary>
        [Obsolete]
        public static List<Dictionary<string, AttributeValue>> ReadList(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return attributes[key].L!.Select(x => x.M!).ToList();
        }

        /// <summary>Read an optional value as a list of objects</summary>
        [Obsolete]
        public static List<TEntity>? ReadListOptional<TEntity>(this Dictionary<string, AttributeValue> attributes, string key)
            where TEntity : IDynamoDbParsableEntity<TEntity>
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.L!.Select(x => TEntity.ParseFromDynamoDb(x.M!)).ToList();
            }
            return null;
        }

        /// <summary>Read an optional value as a list of objects</summary>
        [Obsolete]
        public static List<Dictionary<string, AttributeValue>>? ReadListOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.L!.Select(x => x.M!).ToList();
            }
            return null;
        }

        /// <summary>Read a value as a list of string</summary>
        [Obsolete]
        public static List<string> ReadListString(this Dictionary<string, AttributeValue> attributes, string key)
        {
            var attribute = attributes[key];
            if (attribute.SS != null && attribute.SS.Count != 0)
            {
                return attribute.SS;
            }
            else
            {
                return attribute.L!.Select(x => x.S!).ToList();
            }
        }

        /// <summary>Read an optional value as a list of string</summary>
        [Obsolete]
        public static List<string>? ReadListStringOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                if (attribute.SS != null && attribute.SS.Count != 0)
                {
                    return attribute.SS;
                }
                else
                {
                    return attribute.L!.Select(x => x.S!).ToList();
                }
            }
            return null;
        }

        /// <summary>Read a value as a string</summary>
        [Obsolete]
        public static string ReadString(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return attributes[key].S!;
        }

        /// <summary>Read an optional value as a string</summary>
        [Obsolete]
        public static string? ReadStringOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.S;
            }
            return null;
        }

        /// <summary>Read a value as a GUID</summary>
        [Obsolete]
        public static Guid ReadGuid(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return Guid.Parse(attributes[key].S!);
        }

        /// <summary>Read an optional value as a GUID</summary>
        [Obsolete]
        public static Guid? ReadGuidOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null && attribute.S != null)
            {
                return Guid.Parse(attribute.S);
            }
            return null;
        }

        /// <summary>Read a value as an integer</summary>
        [Obsolete]
        public static int ReadInt(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return int.Parse(attributes[key].N!);
        }

        /// <summary>Read an optional value as an integer</summary>
        [Obsolete]
        public static int? ReadIntOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return int.Parse(attribute.N!);
            }
            return null;
        }

        /// <summary>Read a value as an decimal</summary>
        [Obsolete]
        public static decimal ReadDecimal(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return decimal.Parse(attributes[key].N!);
        }

        /// <summary>Read an optional value as an decimal</summary>
        [Obsolete]
        public static decimal? ReadDecimalOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return decimal.Parse(attribute.N!);
            }
            return null;
        }

        /// <summary>Read a value as a list of doubles</summary>
        [Obsolete]
        public static List<double> ReadListDouble(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return attributes[key].NS!.Select(x => double.Parse(x)).ToList();
        }

        /// <summary>Read an optional value as a list of doubles</summary>
        [Obsolete]
        public static List<double>? ReadListDoubleOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.NS!.Select(x => double.Parse(x)).ToList();
            }
            return null;
        }

        /// <summary>Read a value as a double</summary>
        [Obsolete]
        public static double ReadDouble(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return double.Parse(attributes[key].N!);
        }

        /// <summary>Read an optional value as a double</summary>
        [Obsolete]
        public static double? ReadDoubleOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return double.Parse(attribute.N!);
            }
            return null;
        }

        /// <summary>Read a value as an enumeration</summary>
        [Obsolete]
        public static TEnum ReadEnum<TEnum>(this Dictionary<string, AttributeValue> attributes, string key)
            where TEnum : struct, Enum
        {
            return Enum.Parse<TEnum>(attributes[key].N!);
        }

        /// <summary>Read an optional value as an enumeration</summary>
        [Obsolete]
        public static TEnum? ReadEnumOptional<TEnum>(this Dictionary<string, AttributeValue> attributes, string key)
            where TEnum : struct, Enum
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return Enum.Parse<TEnum>(attribute.N!);
            }
            return null;
        }

        /// <summary>Read an optional value as a boolean</summary>
        [Obsolete]
        public static bool? ReadBoolOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return attribute.BOOL;
            }
            return null;
        }

        /// <summary>Read a value as a date time offset</summary>
        [Obsolete]
        public static DateTimeOffset ReadDateTimeOffset(this Dictionary<string, AttributeValue> attributes, string key)
        {
            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(attributes[key].N!));
        }

        /// <summary>Read an optional value as a date time offset</summary>
        [Obsolete]
        public static DateTimeOffset? ReadDateTimeOffsetOptional(this Dictionary<string, AttributeValue> attributes, string key)
        {
            if (attributes.TryGetValue(key, out var attribute) && attribute != null)
            {
                return DateTimeOffset.FromUnixTimeSeconds(long.Parse(attribute.N!));
            }
            return null;
        }
    }
}
