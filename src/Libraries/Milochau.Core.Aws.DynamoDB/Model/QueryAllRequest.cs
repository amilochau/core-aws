using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>Container for the parameters to the QueryAll operation.</summary>
    public class QueryAllRequest<TEntity>
        where TEntity : class, IDynamoDbQueryableEntity<TEntity>
    {
        /// <inheritdoc cref="AmazonWebServiceRequest.UserId"/>
        public required Guid? UserId { get; set; }

        /// <inheritdoc cref="AmazonDynamoDBRequest.ReturnConsumedCapacity"/>
        public ReturnConsumedCapacity? ReturnConsumedCapacity { get; set; }

        /// <inheritdoc cref="QueryRequest{TEntity}.PartitionKeyCondition"/>
        public required IQueryPartitionKeyConditionExpression PartitionKeyCondition { get; set; }
        /// <inheritdoc cref="QueryRequest{TEntity}.SortKeyCondition"/>
        public IQuerySortKeyConditionExpression? SortKeyCondition { get; set; }

        /// <inheritdoc cref="QueryRequest{TEntity}.Filters"/>
        public IFilterExpression? Filters { get; set; }

        /// <inheritdoc cref="QueryRequest{TEntity}.ConsistentRead"/>
        public bool ConsistentRead { get; set; }
    }
}
