using Microsoft.CodeAnalysis;
using Milochau.Core.Aws.DynamoDB.Abstractions;
using Milochau.Core.Aws.DynamoDB.Generator.Models;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Generator
{
    internal static class SourceGenerationHelper
    {
        public const string DynamoDbTableAttributeName = nameof(DynamoDbTableAttribute);
        public const string DynamoDbProjectionAttributeName = nameof(DynamoDbProjectionAttribute);
        public const string DynamoDbIndexAttributeName = nameof(DynamoDbIndexAttribute);
        public const string DynamoDbNestedAttributeName = nameof(DynamoDbNestedAttribute);

        public const string DynamoDbAttribute_TableNameSuffix = "TableNameSuffix";
        public const string DynamoDbAttribute_IndexName = "IndexName";

        public const string DynamoDbAttributeAttributeName = nameof(DynamoDbAttributeAttribute);
        public const string DynamoDbAttributeAttributeName_PartitionKey = nameof(DynamoDbPartitionKeyAttributeAttribute);
        public const string DynamoDbAttributeAttributeName_SortKey = nameof(DynamoDbSortKeyAttributeAttribute);

        private static string? GetFormatLine(DynamoDbAttributeToGenerate attribute)
        {
            if (attribute.IsDictionary)
            {
                return attribute.AttributeType switch
                {
                    AttributeType.String
                        or AttributeType.Guid
                        or AttributeType.Int
                        or AttributeType.Long
                        or AttributeType.Decimal
                        or AttributeType.Double
                        or AttributeType.Boolean
                        or AttributeType.DateTimeOffset
                    when attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}?.ToDictionary(x => x.Key, x => new AttributeValue(x.Value)))), x => x.Value.M != null)",
                    AttributeType.String
                        or AttributeType.Guid
                        or AttributeType.Int
                        or AttributeType.Long
                        or AttributeType.Decimal
                        or AttributeType.Double
                        or AttributeType.Boolean
                        or AttributeType.DateTimeOffset
                    when !attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}.ToDictionary(x => x.Key, x => new AttributeValue(x.Value)))), x => x.Value.M != null)",
                    AttributeType.Object when attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}?.ToDictionary(x => x.Key, x => new AttributeValue(x.Value.FormatForDynamoDb())))), x => x.Value.M != null)",
                    AttributeType.Object when !attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}.ToDictionary(x => x.Key, x => new AttributeValue(x.Value.FormatForDynamoDb())))), x => x.Value.M != null)",
                    _ => $"// Missing: {attribute.Name}",
                };
            }
            else if (attribute.IsList)
            {
                return attribute.AttributeType switch
                {
                    AttributeType.String
                        or AttributeType.Guid
                        or AttributeType.Int
                        or AttributeType.Long
                        or AttributeType.Decimal
                        or AttributeType.Double
                        or AttributeType.Boolean
                        or AttributeType.DateTimeOffset
                    when attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}?.Select(x => new AttributeValue(x)))), x => x.Value.L != null)",
                    AttributeType.String
                        or AttributeType.Guid
                        or AttributeType.Int
                        or AttributeType.Long
                        or AttributeType.Decimal
                        or AttributeType.Double
                        or AttributeType.Boolean
                        or AttributeType.DateTimeOffset
                    when !attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}.Select(x => new AttributeValue(x)))), x => x.Value.L != null)",
                    AttributeType.Object when attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}?.Select(x => x.FormatForDynamoDb()))), x => x.Value.L != null)",
                    AttributeType.Object when !attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}.Select(x => x.FormatForDynamoDb()))), x => x.Value.L != null)",
                    _ => $"// Missing: {attribute.Name}",
                };
            }
            else
            {
                return attribute.AttributeType switch
                {
                    AttributeType.String => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name})), !string.IsNullOrWhiteSpace({attribute.Name}))",
                    AttributeType.Guid
                        or AttributeType.Int
                        or AttributeType.Long
                        or AttributeType.Decimal
                        or AttributeType.Double
                        or AttributeType.DateTimeOffset
                    when attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name})), {attribute.Name} != null)",
                    AttributeType.Guid
                        or AttributeType.Int
                        or AttributeType.Long
                        or AttributeType.Decimal
                        or AttributeType.Double
                        or AttributeType.DateTimeOffset
                    when !attribute.IsNullable => $".Append(new(\"{attribute.Key}\", new AttributeValue({attribute.Name})))",
                    AttributeType.Boolean => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue({attribute.Name})), {attribute.Name} == true)",
                    AttributeType.Enum when attribute.IsNullable => $".AppendIf(new(\"{attribute.Key}\", new AttributeValue(Convert.ToInt32({attribute.Name}))), {attribute.Name} != null)",
                    AttributeType.Enum when !attribute.IsNullable => $".Append(new(\"{attribute.Key}\", new AttributeValue(Convert.ToInt32({attribute.Name}))))",
                    AttributeType.Object when attribute.IsNullable => $".Append(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}?.FormatForDynamoDb())))",
                    AttributeType.Object when !attribute.IsNullable => $".Append(new(\"{attribute.Key}\", new AttributeValue({attribute.Name}.FormatForDynamoDb())))",
                    AttributeType.DateTime
                        or _ => $"// Missing: {attribute.Name}",
                };
            }
        }

        private static string? GetParseLine(DynamoDbAttributeToGenerate attribute)
        {
            if (attribute.IsDictionary)
            {
                return attribute.AttributeType switch
                {
                    AttributeType.String => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.S!)).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Guid => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, System.Guid>(x.Key, System.Guid.Parse(x.Value.S!))).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Int => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, int>(x.Key, int.Parse(x.Value.N!))).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Long => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, long>(x.Key, long.Parse(x.Value.N!))).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Decimal => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, decimal>(x.Key, decimal.Parse(x.Value.N!))).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Double => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, double>(x.Key, double.Parse(x.Value.N!))).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Boolean => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, bool>(x.Key, x.Value.BOOL!)).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.DateTimeOffset => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, DateTimeOffset>(x.Key, DateTimeOffset.FromUnixTimeSeconds(long.Parse(x.Value.N!)))).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Enum => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, {attribute.Type}>(x.Key, System.Enum.Parse<{attribute.Type}>(x.Value.N!))).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Object => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.Select(x => new KeyValuePair<string, {attribute.Type}>(x.Key, {attribute.Type}.ParseFromDynamoDb(x.Value.M!))).ToDictionary()" + (attribute.IsNullable ? "" : " ?? []"),
                    _ => $"// Missing: {attribute.Name}",
                };
            }
            else if (attribute.IsList)
            {
                return attribute.AttributeType switch
                {
                    AttributeType.String => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => x.S!)?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Guid => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => System.Guid.Parse(x.S!))?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Int => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => int.Parse(x.N!))?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Long => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => long.Parse(x.N!))?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Decimal => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => decimal.Parse(x.N!))?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Double => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => double.Parse(x.N!))?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Boolean => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => x.BOOL!)?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.DateTimeOffset => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => DateTimeOffset.FromUnixTimeSeconds(long.Parse(x.N!)))?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Enum => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => System.Enum.Parse<{attribute.Type}>(x.N!))?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    AttributeType.Object => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.L?.Select(x => {attribute.Type}.ParseFromDynamoDb(x.M!))?.ToList()" + (attribute.IsNullable ? "" : " ?? []"),
                    _ => $"// Missing: {attribute.Name}",
                };
            }
            else
            {
                return attribute.AttributeType switch
                {
                    AttributeType.String when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.S",
                    AttributeType.String when !attribute.IsNullable => $"{attribute.Name} = attributes[\"{attribute.Key}\"].S!",
                    AttributeType.Guid when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.S?.ApplyOrDefault(System.Guid.Parse)",
                    AttributeType.Guid when !attribute.IsNullable => $"{attribute.Name} = System.Guid.Parse(attributes[\"{attribute.Key}\"].S!)",
                    AttributeType.Int when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.N?.ApplyOrDefault(int.Parse)",
                    AttributeType.Int when !attribute.IsNullable => $"{attribute.Name} = int.Parse(attributes[\"{attribute.Key}\"].N!)",
                    AttributeType.Long when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.N?.ApplyOrDefault(long.Parse)",
                    AttributeType.Long when !attribute.IsNullable => $"{attribute.Name} = long.Parse(attributes[\"{attribute.Key}\"].N!)",
                    AttributeType.Decimal when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.N?.ApplyOrDefault(decimal.Parse)",
                    AttributeType.Decimal when !attribute.IsNullable => $"{attribute.Name} = decimal.Parse(attributes[\"{attribute.Key}\"].N!)",
                    AttributeType.Double when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.N?.ApplyOrDefault(double.Parse)",
                    AttributeType.Double when !attribute.IsNullable => $"{attribute.Name} = double.Parse(attributes[\"{attribute.Key}\"].N!)",
                    AttributeType.Boolean => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.BOOL" + (attribute.IsNullable ? "" : " ?? false"),
                    AttributeType.DateTimeOffset when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.N?.ApplyOrDefault(x => DateTimeOffset.FromUnixTimeSeconds(long.Parse(x)))",
                    AttributeType.DateTimeOffset when !attribute.IsNullable => $"{attribute.Name} = DateTimeOffset.FromUnixTimeSeconds(long.Parse(attributes[\"{attribute.Key}\"].N!))",
                    AttributeType.Enum when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.N?.ApplyOrDefault(System.Enum.Parse<{attribute.Type}>)",
                    AttributeType.Enum when !attribute.IsNullable => $"{attribute.Name} = System.Enum.Parse<{attribute.Type}>(attributes[\"{attribute.Key}\"].N!)",
                    AttributeType.Object when attribute.IsNullable => $"{attribute.Name} = attributes.GetValueOrDefault(\"{attribute.Key}\")?.M?.ApplyOrDefault({attribute.Type}.ParseFromDynamoDb)",
                    AttributeType.Object when !attribute.IsNullable => $"{attribute.Name} = {attribute.Type}.ParseFromDynamoDb(attributes[\"{attribute.Key}\"].M!)",
                    _ => $"// Missing: {attribute.Name}",
                };
            }
        }

        private static string? GetPartialClassInterfaces(DynamoDbClassToGenerate dynamoDbClassToGenerate)
        {
            var c = dynamoDbClassToGenerate.Class;
            return dynamoDbClassToGenerate.Type switch
            {
                ClassType.Table => $"IDynamoDbQueryableEntity<{c}>, IDynamoDbGettableEntity<{c}>, IDynamoDbPutableEntity<{c}>, IDynamoDbDeletableEntity<{c}>, IDynamoDbUpdatableEntity<{c}>",
                ClassType.Index => $"IDynamoDbQueryableEntity<{c}>",
                ClassType.Projection when string.IsNullOrEmpty(dynamoDbClassToGenerate.IndexName) => $"IDynamoDbQueryableEntity<{c}>, IDynamoDbGettableEntity<{c}>",
                ClassType.Projection when !string.IsNullOrEmpty(dynamoDbClassToGenerate.IndexName) => $"IDynamoDbQueryableEntity<{c}>",
                ClassType.Nested => $"IDynamoDbParsableEntity<{c}>, IDynamoDbFormattableEntity",
                _ => null
            };
        }

        public static string GeneratePartialClass(DynamoDbClassToGenerate dynamoDbClassToGenerate)
        {
            var interfaces = GetPartialClassInterfaces(dynamoDbClassToGenerate);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"#nullable enable
using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace {dynamoDbClassToGenerate.Namespace}
{{
    public partial class {dynamoDbClassToGenerate.Class}{(interfaces != null ? $" : {interfaces}" : "")}
    {{
");
            if (!string.IsNullOrEmpty(dynamoDbClassToGenerate.TableNameSuffix))
            {
                stringBuilder.AppendLine($@"        public static string TableName {{ get; }} = $""{{EnvironmentVariables.ConventionPrefix}}-table-{dynamoDbClassToGenerate.TableNameSuffix}"";");
            }
            if (!string.IsNullOrEmpty(dynamoDbClassToGenerate.IndexName))
            {
                stringBuilder.AppendLine($@"        public static string IndexName {{ get; }} = $""{dynamoDbClassToGenerate.IndexName}"";");
            }

            // Projection helpers
            if (dynamoDbClassToGenerate.Type == ClassType.Projection)
            {
                var projectedAttributes = dynamoDbClassToGenerate.DynamoDbAttributes.Aggregate("", (acc, newItem) => acc + ", " + $"\"{newItem}\"") ?? "";

                stringBuilder.AppendLine($@"        public static IEnumerable<string>? ProjectedAttributes {{ get; }} = [{projectedAttributes}];");
            }

            // Partition and Sort key helpers
            if (dynamoDbClassToGenerate.Type == ClassType.Table || dynamoDbClassToGenerate.Type == ClassType.Index || dynamoDbClassToGenerate.Type == ClassType.Projection)
            {
                var partitionKey = dynamoDbClassToGenerate.DynamoDbAttributes.FirstOrDefault(x => x.AttributeCategory == AttributeCategory.Partition);
                if (partitionKey != default)
                {
                    stringBuilder.AppendLine($@"        public static string PartitionKey {{ get; }} = ""{partitionKey.Key}"";");
                }
                var sortKey = dynamoDbClassToGenerate.DynamoDbAttributes.FirstOrDefault(x => x.AttributeCategory == AttributeCategory.Sort);
                if (sortKey != default)
                {
                    stringBuilder.AppendLine($@"        public static string? SortKey {{ get; }} = ""{sortKey.Key}"";");
                }
            }

            // Attribute key constants
            foreach (var attribute in dynamoDbClassToGenerate.DynamoDbAttributes)
            {
                stringBuilder.AppendLine($@"        public const string K_{attribute.Name} = ""{attribute.Key}"";");
            }

            if (dynamoDbClassToGenerate.Type == ClassType.Table || dynamoDbClassToGenerate.Type == ClassType.Nested)
            {
                stringBuilder.Append($@"
        public Dictionary<string, AttributeValue> FormatForDynamoDb()
        {{
            return new Dictionary<string, AttributeValue>()
");
                foreach (var formatLine in dynamoDbClassToGenerate.DynamoDbAttributes.Select(GetFormatLine).Where(x => x != null))
                {
                    stringBuilder.AppendLine($"                {formatLine}");
                }
                stringBuilder.Append($@"
                .ToDictionary();
        }}
");
            }

            stringBuilder.Append($@"
        public static {dynamoDbClassToGenerate.Class} ParseFromDynamoDb(Dictionary<string, AttributeValue> attributes)
        {{
            return new {dynamoDbClassToGenerate.Class}
            {{
");
                foreach (var parseLine in dynamoDbClassToGenerate.DynamoDbAttributes.Select(GetParseLine).Where(x => x != null))
                {
                    stringBuilder.AppendLine($"                {parseLine},");
                }
                stringBuilder.Append($@"
            }};
        }}
");

            stringBuilder.Append($@"
    }}
}}
");

            return stringBuilder.ToString();
        }
    }
}
