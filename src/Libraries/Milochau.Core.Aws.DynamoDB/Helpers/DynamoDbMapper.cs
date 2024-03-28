using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Mapper for DynamoDB</summary>
    public static class DynamoDbMapper
    {
        /// <summary>Try create an <see cref="AttributeValue"/></summary>
        public static bool TryCreateAttributeValue(object? value, [NotNullWhen(true)] out AttributeValue? attributeValue)
        {
            attributeValue = CreateAttributeValue(value);
            return attributeValue != null;
        }

        /// <summary>Create an <see cref="AttributeValue"/> when possible</summary>
        public static AttributeValue? CreateAttributeValue(object? value)
        {
            if (value == null)
            {
                return null;
            }
            if (value == default)
            {
                return null;
            }

            var type = value.GetType();

            // @todo Don't add empty/default values

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    if (type.IsEnum)
                    {
                        return new AttributeValue
                        {
                            N = $"{Convert.ToInt32(value)}"
                        };
                    }
                    else
                    {
                        return new AttributeValue
                        {
                            N = $"{value}"
                        };
                    }
                case TypeCode.Boolean:
                    var boolValue = (bool)value;
                    if (!boolValue)
                    {
                        return null; // Don't store 'false' value
                    }
                    return new AttributeValue
                    {
                        BOOL = boolValue
                    };
                case TypeCode.String:
                    var stringValue = (string)value;
                    if (string.IsNullOrWhiteSpace(stringValue))
                    {
                        return null; // Don't store empty/whitespace strings
                    }
                    return new AttributeValue
                    {
                        S = stringValue.Trim()
                    };
                case TypeCode.Char:
                    var charValue = $"{(char)value}";
                    if (string.IsNullOrWhiteSpace(charValue))
                    {
                        return null; // Don't store empty/whitespace strings
                    }
                    return new AttributeValue
                    {
                        S = charValue
                    };
                case TypeCode.DateTime: // DateTime
                    var dateTime = (DateTime)value;
                    if (dateTime == default)
                    {
                        return null; // Default DateTime can't be converted to Unix time
                    }
                    return new AttributeValue
                    {
                        N = $"{((DateTimeOffset)dateTime).ToUnixTimeSeconds()}"
                    };
                case TypeCode.Object when value.GetType() == typeof(Guid): // Guid
                    return new AttributeValue
                    {
                        S = ((Guid)value).ToString("N")
                    };
                case TypeCode.Object when value.GetType() == typeof(DateTimeOffset): // DateTimeOffset
                    return new AttributeValue
                    {
                        N = $"{((DateTimeOffset)value).ToUnixTimeSeconds()}"
                    };
                case TypeCode.Object when type == typeof(IDictionary) || type.GetInterfaces().Any(t => t == typeof(IDictionary)): // IDictionary (to be tested before IEnumerable, as dictionaries are enumerables)
                    var valueAsDictionary = (IDictionary)value;
                    var dictionaryAttributes = new Dictionary<string, AttributeValue>();
                    foreach (var key in valueAsDictionary.Keys)
                    {
                        if (TryCreateAttributeValue(valueAsDictionary[key], out var attributeValue))
                        {
                            dictionaryAttributes.Add((string)key, attributeValue);
                        }
                    }
                    if (dictionaryAttributes.Count == 0)
                    {
                        return null; // Don't store empty dictionaries
                    }
                    return new AttributeValue
                    {
                        M = dictionaryAttributes
                    };
                case TypeCode.Object when type.IsArray: // Array
                    var valueAsArray = (Array)value;
                    var arrayAttributes = new List<AttributeValue>();
                    foreach (var item in valueAsArray)
                    {
                        if (TryCreateAttributeValue(item, out var attributeValue))
                        {
                            arrayAttributes.Add(attributeValue);
                        }
                    }
                    if (arrayAttributes.Count == 0)
                    {
                        return null; // Don't store empty lists
                    }
                    return new AttributeValue
                    {
                        L = arrayAttributes
                    };
                case TypeCode.Object when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || type == typeof(IEnumerable) || type.GetInterfaces().Any(t => t == typeof(IEnumerable)): // List
                    var valueAsEnumerable = (IEnumerable)value;
                    var listAttributes = new List<AttributeValue>();
                    foreach (var item in valueAsEnumerable)
                    {
                        if (TryCreateAttributeValue(item, out var attributeValue))
                        {
                            listAttributes.Add(attributeValue);
                        }
                    }
                    if (listAttributes.Count == 0)
                    {
                        return null; // Don't store empty lists
                    }
                    return new AttributeValue
                    {
                        L = listAttributes
                    };
                case TypeCode.Object: // Fallback: use object properties in a map
                    var objectAttributes = GetAttributes(type, value);
                    if (objectAttributes.Count == 0)
                    {
                        return null; // Don't store empty objects
                    }
                    return new AttributeValue
                    {
                        M = objectAttributes
                    };
                case TypeCode.Empty:
                case TypeCode.DBNull:
                default:
                    break;
            }

            return null; // Fallback: don't store properties that can't be converted
        }

        /// <summary>Get DynamoDB attributes</summary>
        public static Dictionary<string, AttributeValue> GetAttributes([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, object value)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var attributes = new Dictionary<string, AttributeValue>();

            foreach (var property in properties)
            {
                var attribute = (DynamoDbAttributeAttribute?)property.GetCustomAttribute(typeof(DynamoDbAttributeAttribute));
                if (attribute == null)
                {
                    continue;
                }

                var dynamoDbAttributeValue = CreateAttributeValue(property.GetValue(value));
                if (dynamoDbAttributeValue == null)
                {
                    continue;
                }

                attributes.Add(attribute.Key, dynamoDbAttributeValue);
            }

            return attributes;
        }
    }
}
