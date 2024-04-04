using Milochau.Core.Aws.DynamoDB.Generator.Helpers;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Generator
{
    public readonly record struct DynamoDbToGenerate
    {
        public readonly string Namespace;
        public readonly string Class;
        public readonly string? TableNameSuffix;
        public readonly string? IndexName;
        public readonly bool IsParsable;
        public readonly bool IsFormattable;
        public readonly bool IsProjectable;

        public readonly ImmutableEquatableArray<DynamoDbAttributeToGenerate> DynamoDbAttributes;

        public DynamoDbToGenerate(string @namespace, string @class, string? tableNameSuffix, string? indexName, bool isParsable, bool isFormattable, bool isProjectable, List<DynamoDbAttributeToGenerate> dynamoDbAttributes)
        {
            Namespace = @namespace;
            Class = @class;
            TableNameSuffix = tableNameSuffix;
            IndexName = indexName;
            IsParsable = isParsable;
            IsFormattable = isFormattable;
            IsProjectable = isProjectable;
            DynamoDbAttributes = new(dynamoDbAttributes);
        }
    }
}
