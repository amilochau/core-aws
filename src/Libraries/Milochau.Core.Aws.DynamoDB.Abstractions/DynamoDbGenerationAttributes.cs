using System;

namespace Milochau.Core.Aws.DynamoDB.Abstractions
{
    /// <summary>Attribute used to generate code for a DynamoDB table</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DynamoDbTableAttribute(string tableNameSuffix) : Attribute
    {
        /// <summary>Table name suffix</summary>
        public string TableNameSuffix { get; } = tableNameSuffix;
    }

    /// <summary>Attribute used to generate code for a DynamoDB projection</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DynamoDbProjectionAttribute(string tableNameSuffix) : Attribute
    {
        /// <summary>Table name suffix</summary>
        public string TableNameSuffix { get; } = tableNameSuffix;

        /// <summary>Index name</summary>
        public string? IndexName { get; set; }
    }

    /// <summary>Attribute used to generate code for a DynamoDB index</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DynamoDbIndexAttribute(string tableNameSuffix, string indexName) : Attribute
    {
        /// <summary>Table name suffix</summary>
        public string TableNameSuffix { get; } = tableNameSuffix;

        /// <summary>Index name</summary>
        public string IndexName { get; } = indexName;
    }

    /// <summary>Attribute used to generate code for a DynamoDB nested class</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DynamoDbNestedAttribute : Attribute
    {
    }

    /// <summary>Attribute used to generate code for a DynamoDB attribute</summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DynamoDbAttributeAttribute(string key) : Attribute
    {
        /// <summary>Attribute key</summary>
        public string Key { get; } = key;

        /// <summary>Use default initializer in case of missing key from DynamoDB</summary>
        public bool UseDefaultInitializer { get; set; } = false;
    }

    /// <summary>Attribute used to generate code for a DynamoDB Partition Key attribute</summary>
    public sealed class DynamoDbPartitionKeyAttributeAttribute(string key) : DynamoDbAttributeAttribute(key)
    {
    }

    /// <summary>Attribute used to generate code for a DynamoDB Sort Key attribute</summary>
    public sealed class DynamoDbSortKeyAttributeAttribute(string key) : DynamoDbAttributeAttribute(key)
    {
    }
}
