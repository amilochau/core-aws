using System;

namespace Milochau.Core.Aws.DynamoDB.Abstractions
{
    /// <summary>Attribute used to generate code for a DynamoDB table</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DynamoDbTableAttribute(string tableNameSuffix, string? applicationName = null) : Attribute
    {
        /// <summary>Table name suffix</summary>
        public string TableNameSuffix { get; } = tableNameSuffix;

        /// <summary>Application name</summary>
        public string? ApplicationName { get; } = applicationName;
    }

    /// <summary>Attribute used to generate code for a DynamoDB projection</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DynamoDbProjectionAttribute(string tableNameSuffix, string? applicationName = null) : Attribute
    {
        /// <summary>Table name suffix</summary>
        public string TableNameSuffix { get; } = tableNameSuffix;

        /// <summary>Application name</summary>
        public string? ApplicationName { get; } = applicationName;

        /// <summary>Index name</summary>
        public string? IndexName { get; set; }
    }

    /// <summary>Attribute used to generate code for a DynamoDB index</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DynamoDbIndexAttribute(string tableNameSuffix, string indexName, string? applicationName = null) : Attribute
    {
        /// <summary>Table name suffix</summary>
        public string TableNameSuffix { get; } = tableNameSuffix;

        /// <summary>Application name</summary>
        public string? ApplicationName { get; } = applicationName;

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

        /// <summary>Use set (<c>AttributeValue.SS</c> and <c>AttributeValue.NS</c>) if the attribute is an enumerable</summary>
        public bool UseSet { get; set; } = false;
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
