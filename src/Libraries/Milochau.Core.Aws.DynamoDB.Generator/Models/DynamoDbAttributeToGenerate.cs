namespace Milochau.Core.Aws.DynamoDB.Generator.Models
{
    internal readonly record struct DynamoDbAttributeToGenerate
    {
        public readonly string Type;
        public readonly string Name;
        public readonly string Key;
        public readonly AttributeType AttributeType;
        public readonly AttributeCategory AttributeCategory;
        public readonly bool IsNullable;
        public readonly bool UseDefaultInitializer;

        public readonly bool IsList;
        public readonly bool IsDictionary;

        public DynamoDbAttributeToGenerate(string type, string name, string key, AttributeType attributeType, AttributeCategory attributeCategory, bool isNullable, bool useDefaultInitializer, bool isList, bool isDictionary)
        {
            Type = type;
            Name = name;
            Key = key;
            AttributeType = attributeType;
            AttributeCategory = attributeCategory;
            IsNullable = isNullable;
            UseDefaultInitializer = useDefaultInitializer;
            IsList = isList;
            IsDictionary = isDictionary;
        }
    }
}
