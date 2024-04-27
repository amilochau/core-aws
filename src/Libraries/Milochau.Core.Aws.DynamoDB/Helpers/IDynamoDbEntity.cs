using Milochau.Core.Aws.DynamoDB.Model;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>DynamoDB parsable entity</summary>
    public interface IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbParsableEntity<TEntity>
    {
        /// <summary>Parse from DynamoDB to entity</summary>
        abstract static TEntity ParseFromDynamoDb(Dictionary<string, AttributeValue> attributes);
    }

    /// <summary>DynamoDB formattable entity</summary>
    public interface IDynamoDbFormattableEntity
    {
        /// <summary>Format entity for DynamoDB</summary>
        Dictionary<string, AttributeValue> FormatForDynamoDb();
    }

    /// <summary>DynamoDB gettable entity</summary>
    public interface IDynamoDbGettableEntity<TEntity> : IDynamoDbParsableEntity<TEntity>
        where TEntity : class, IDynamoDbGettableEntity<TEntity>
    {
        /// <summary>Name of the DynamoDB table</summary>
        static abstract string TableName { get; }

        /// <summary>List of projected attributes</summary>
        static virtual IEnumerable<string>? ProjectedAttributes { get; } = null;

        /// <summary>Partition key</summary>
        static abstract string PartitionKey { get; }

        /// <summary>Sort key</summary>
        static virtual string? SortKey { get; } = null;
    }

    /// <summary>DynamoDB queryable entity</summary>
    public interface IDynamoDbQueryableEntity<TEntity> : IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbQueryableEntity<TEntity>
    {
        /// <summary>Name of the DynamoDB table</summary>
        static abstract string TableName { get; }

        /// <summary>Name of the DynamoDB index</summary>
        static virtual string? IndexName { get; }

        /// <summary>List of projected attributes</summary>
        static virtual IEnumerable<string>? ProjectedAttributes { get; } = null;
    }

    /// <summary>DynamoDB scanable entity</summary>
    public interface IDynamoDbScanableEntity<TEntity> : IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbScanableEntity<TEntity>
    {
        /// <summary>Name of the DynamoDB table</summary>
        static abstract string TableName { get; }

        /// <summary>Name of the DynamoDB index</summary>
        static virtual string? IndexName { get; }

        /// <summary>List of projected attributes</summary>
        static virtual IEnumerable<string>? ProjectedAttributes { get; } = null;
    }

    /// <summary>DynamoDB putable entity</summary>
    public interface IDynamoDbPutableEntity<TEntity> : IDynamoDbFormattableEntity, IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbPutableEntity<TEntity>
    {
        /// <summary>Name of the DynamoDB table</summary>
        static abstract string TableName { get; }
    }

    /// <summary>DynamoDB deletable entity</summary>
    public interface IDynamoDbDeletableEntity<TEntity> : IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbDeletableEntity<TEntity>
    {
        /// <summary>Name of the DynamoDB table</summary>
        static abstract string TableName { get; }

        /// <summary>Partition key</summary>
        static abstract string PartitionKey { get; }

        /// <summary>Sort key</summary>
        static virtual string? SortKey { get; } = null;
    }

    /// <summary>DynamoDB updatable entity</summary>
    public interface IDynamoDbUpdatableEntity<TEntity> : IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbUpdatableEntity<TEntity>
    {
        /// <summary>Name of the DynamoDB table</summary>
        static abstract string TableName { get; }

        /// <summary>Partition key</summary>
        static abstract string PartitionKey { get; }

        /// <summary>Sort key</summary>
        static virtual string? SortKey { get; } = null;
    }

    /// <summary>DynamoDB batch writable entity</summary>
    public interface IDynamoDbBatchWritableEntity<TEntity> : IDynamoDbFormattableEntity, IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbBatchWritableEntity<TEntity>
    {
        /// <summary>Name of the DynamoDB table</summary>
        static abstract string TableName { get; }

        /// <summary>Partition key</summary>
        static abstract string PartitionKey { get; }

        /// <summary>Sort key</summary>
        static virtual string? SortKey { get; } = null;
    }
}
