using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>
    /// Interface for DynamoDB entity
    /// </summary>
    public interface IDynamoDbEntity<TEntity> : IDynamoDbParsableEntity<TEntity>
        where TEntity: IDynamoDbEntity<TEntity>
    {
        /// <summary>Format entity for DynamoDB</summary>
        Dictionary<string, AttributeValue> FormatForDynamoDb();
    }

    /// <summary>
    /// Interface for DynamoDB parsable entity
    /// </summary>
    public interface IDynamoDbParsableEntity<TEntity>
        where TEntity : IDynamoDbParsableEntity<TEntity>
    {
        /// <summary>Parse from DynamoDB to entity</summary>
        abstract static TEntity ParseFromDynamoDb(Dictionary<string, AttributeValue> attributes);
    }
}
