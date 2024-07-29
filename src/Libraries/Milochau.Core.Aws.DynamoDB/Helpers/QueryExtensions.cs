using Milochau.Core.Aws.DynamoDB.Model;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Extension methods for the <c>QueryAsync</c> operation</summary>
    public static class QueryExtensions
    {
        /// <summary>Scan all entities from a DynamoDB table, under all partitions</summary>
        public static async IAsyncEnumerable<TEntity> ScanAllAsync<TEntity>(this IAmazonDynamoDB amazonDynamoDB,
            Guid? userId,
            IFilterExpression? filterExpression,
            [EnumeratorCancellation] CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbScanableEntity<TEntity>
        {
            var request = new ScanRequest<TEntity>
            {
                UserId = userId,
                Filters = filterExpression,
            };

            do
            {
                var response = await amazonDynamoDB.ScanAsync(request, cancellationToken);
                if (response.Entities == null || response.Entities.Count == 0)
                {
                    break;
                }
                foreach (var entity in response.Entities)
                {
                    yield return entity;
                }
                request.ExclusiveStartKey = response.LastEvaluatedKey;
            } while (request.ExclusiveStartKey != null);
        }

        /// <summary>Query all entities from a DynamoDB table, under a single partition</summary>
        public static async IAsyncEnumerable<TEntity> QueryAllAsync<TEntity>(this IAmazonDynamoDB amazonDynamoDB,
            Guid? userId,
            IQueryPartitionKeyConditionExpression partitionKeyCondition,
            IQuerySortKeyConditionExpression? sortKeyCondition,
            IFilterExpression? filterExpression,
            [EnumeratorCancellation] CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbQueryableEntity<TEntity>
        {
            var request = new QueryRequest<TEntity>
            {
                UserId = userId,
                PartitionKeyCondition = partitionKeyCondition,
                SortKeyCondition = sortKeyCondition,
                Filters = filterExpression,
            };

            do
            {
                var response = await amazonDynamoDB.QueryAsync(request, cancellationToken);
                if (response.Entities == null || response.Entities.Count == 0)
                {
                    break;
                }
                foreach (var entity in response.Entities)
                {
                    yield return entity;
                }
                request.ExclusiveStartKey = response.LastEvaluatedKey;
            } while (request.ExclusiveStartKey != null);
        }

        /// <summary>Update all entities from a DynamoDB table, under all partitions</summary>
        public static async Task UpdateAllAsync<TScanableEntity, TUpdatableEntity>(this IAmazonDynamoDB amazonDynamoDB,
            Guid? userId,
            IFilterExpression? filterExpression,
            Func<TScanableEntity, UpdateItemRequest<TUpdatableEntity>> updateItemRequestFunction,
            CancellationToken cancellationToken)
            where TScanableEntity : class, IDynamoDbScanableEntity<TScanableEntity>
            where TUpdatableEntity : class, IDynamoDbUpdatableEntity<TUpdatableEntity>
        {
            var entities = amazonDynamoDB.ScanAllAsync<TScanableEntity>(userId, filterExpression, cancellationToken);

            await foreach (TScanableEntity entity in entities)
            {
                try // We can't use BatchWriteItem to update items
                {
                    await amazonDynamoDB.UpdateItemAsync(updateItemRequestFunction(entity), cancellationToken);
                }
                catch (Exception)
                {
                    // Don't throw if exceptions happen
                }
            }
        }

        /// <summary>Update all entities from a DynamoDB table, under a single partition</summary>
        public static async Task UpdateAllAsync<TQueryableEntity, TUpdatableEntity>(this IAmazonDynamoDB amazonDynamoDB,
            Guid? userId,
            IQueryPartitionKeyConditionExpression partitionKeyCondition,
            IQuerySortKeyConditionExpression? sortKeyCondition,
            IFilterExpression? filterExpression,
            Func<TQueryableEntity, UpdateItemRequest<TUpdatableEntity>> updateItemRequestFunction,
            CancellationToken cancellationToken)
            where TQueryableEntity : class, IDynamoDbQueryableEntity<TQueryableEntity>
            where TUpdatableEntity : class, IDynamoDbUpdatableEntity<TUpdatableEntity>
        {
            var entities = amazonDynamoDB.QueryAllAsync<TQueryableEntity>(userId, partitionKeyCondition, sortKeyCondition, filterExpression, cancellationToken);

            await foreach (TQueryableEntity entity in entities)
            {
                try // We can't use BatchWriteItem to update items
                {
                    await amazonDynamoDB.UpdateItemAsync(updateItemRequestFunction(entity), cancellationToken);
                }
                catch (Exception)
                {
                    // Don't throw if exceptions happen
                }
            }
        }

        /// <summary>Write all entities from a DynamoDB table, under all partitions</summary>
        public static async Task WriteAllAsync<TScanableEntity, TDeleteEntity>(this IAmazonDynamoDB amazonDynamoDB,
            Guid? userId,
            IFilterExpression? filterExpression,
            Func<TScanableEntity, WriteRequest<TDeleteEntity>> deleteRequestFunction,
            CancellationToken cancellationToken)
            where TScanableEntity : class, IDynamoDbScanableEntity<TScanableEntity>
            where TDeleteEntity : class, IDynamoDbBatchWritableEntity<TDeleteEntity>
        {
            var entities = amazonDynamoDB.ScanAllAsync<TScanableEntity>(userId, filterExpression, cancellationToken);

            List<WriteRequest<TDeleteEntity>> writeRequests = [];
            await foreach (TScanableEntity entity in entities)
            {
                writeRequests.Add(deleteRequestFunction(entity));
            }
        }

        /// <summary>Write all entities from a DynamoDB table, under a single partition</summary>
        public static async Task WriteAllAsync<TQueryableEntity, TDeleteEntity>(this IAmazonDynamoDB amazonDynamoDB,
            Guid? userId,
            IQueryPartitionKeyConditionExpression partitionKeyCondition,
            IQuerySortKeyConditionExpression? sortKeyCondition,
            IFilterExpression? filterExpression,
            Func<TQueryableEntity, WriteRequest<TDeleteEntity>> deleteRequestFunction,
            CancellationToken cancellationToken)
            where TQueryableEntity : class, IDynamoDbQueryableEntity<TQueryableEntity>
            where TDeleteEntity: class, IDynamoDbBatchWritableEntity<TDeleteEntity>
        {
            var entities = amazonDynamoDB.QueryAllAsync<TQueryableEntity>(userId, partitionKeyCondition, sortKeyCondition, filterExpression, cancellationToken);

            List<WriteRequest<TDeleteEntity>> writeRequests = [];
            await foreach (TQueryableEntity entity in entities)
            {
                writeRequests.Add(deleteRequestFunction(entity));
            }
        }
    }
}
