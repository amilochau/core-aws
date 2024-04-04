using Microsoft.VisualBasic;
using Milochau.Core.Aws.DynamoDB.Model;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>DynamoDB entity</summary>
    public abstract class DynamoDbEntity<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEntity>
        where TEntity : DynamoDbEntity<TEntity>
    {
        /// <inheritdoc/>
        public static IEnumerable<string>? GetProjectedAttributes()
        {
            var customAttributes = typeof(TEntity)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(x => (DynamoDbAttributeAttribute?)x.GetCustomAttribute(typeof(DynamoDbAttributeAttribute)))
                .Where(x => x != null)
                .Select(x => x!.Key);
            return !customAttributes.Any() ? null : customAttributes;
        }

        /// <inheritdoc/>
        public virtual Dictionary<string, AttributeValue> FormatForDynamoDb()
        {
            return DynamoDbMapper.GetAttributes(typeof(TEntity), this);
        }
    }

    /// <summary>DynamoDB entity</summary>
    public interface IDynamoDbEntity<TEntity> : IDynamoDbFormattableEntity, IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbEntity<TEntity>
    {
    }

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
        static virtual IEnumerable<string>? GetProjectedAttributes() => null;
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
    }

    /// <summary>DynamoDB updatable entity</summary>
    public interface IDynamoDbUpdatableEntity<TEntity> : IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbUpdatableEntity<TEntity>
    {
        /// <summary>Name of the DynamoDB table</summary>
        static abstract string TableName { get; }
    }
}
