namespace Milochau.Core.Aws.DynamoDB.Generator
{
    public readonly record struct DynamoDbAttributeToGenerate
    {
        public readonly string Type;
        public readonly string Name;
        public readonly string Key;
        public readonly AttributeType AttributeType;
        public readonly bool IsNullable;

        public readonly bool IsList;
        public readonly bool IsDictionary;

        public DynamoDbAttributeToGenerate(string type, string name, string key, AttributeType attributeType, bool isNullable, bool isList, bool isDictionary)
        {
            Type = type;
            Name = name;
            Key = key;
            AttributeType = attributeType;
            IsNullable = isNullable;
            IsList = isList;
            IsDictionary = isDictionary;
        }
    }
}
