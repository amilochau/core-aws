using Milochau.Core.Aws.DynamoDB.Generator.Helpers;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Generator.Models
{
    internal readonly record struct DynamoDbClassToGenerate
    {
        public readonly string Namespace;
        public readonly string Class;
        public readonly string? TableNameSuffix;
        public readonly string? ApplicationName;
        public readonly string? IndexName;
        public readonly ClassType Type;

        public readonly ImmutableEquatableArray<DynamoDbAttributeToGenerate> DynamoDbAttributes;
        public readonly ImmutableEquatableArray<DiagnosticInfo> Diagnostics;

        public DynamoDbClassToGenerate(string @namespace, string @class, string? tableNameSuffix, string? applicationName, string? indexName, ClassType type, List<DynamoDbAttributeToGenerate> dynamoDbAttributes, List<DiagnosticInfo> diagnostics)
        {
            Namespace = @namespace;
            Class = @class;
            TableNameSuffix = tableNameSuffix;
            ApplicationName = applicationName;
            IndexName = indexName;
            Type = type;
            DynamoDbAttributes = new(dynamoDbAttributes);
            Diagnostics = new(diagnostics);
        }
    }
}
