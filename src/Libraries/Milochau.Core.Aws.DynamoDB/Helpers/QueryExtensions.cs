using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Extension methods for the <c>Query</c> operation</summary>
    public static class QueryExtensions
    {
        /// <summary>Query all entities from a DynamoDB table, under a single partition</summary>
        public static async Task<QueryAllResponse<TEntity>> QueryAllAsync<TEntity>(this IAmazonDynamoDB amazonDynamoDB,
            QueryAllRequest<TEntity> request,
            CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbQueryableEntity<TEntity>
        {
            var queryRequest = new QueryRequest<TEntity>
            {
                UserId = request.UserId,
                ReturnConsumedCapacity = request.ReturnConsumedCapacity,
                PartitionKeyCondition = request.PartitionKeyCondition,
                SortKeyCondition = request.SortKeyCondition,
                Filters = request.Filters,
                ConsistentRead = request.ConsistentRead,
            };

            var response = new QueryAllResponse<TEntity>();
            do
            {
                var queryResponse = await amazonDynamoDB.QueryAsync(queryRequest, cancellationToken);
                if (queryResponse.Items == null || queryResponse.Entities == null || queryResponse.Entities.Count == 0)
                {
                    break;
                }

                response.Entities.AddRange(queryResponse.Entities);
                response.Items.AddRange(queryResponse.Items);
                response.Count += queryResponse.Count ?? 0;
                response.ScannedCount += queryResponse.ScannedCount ?? 0;

                queryRequest.ExclusiveStartKey = queryResponse.LastEvaluatedKey;
            } while (queryRequest.ExclusiveStartKey != null);

            return response;
        }

        /// <summary>Update all entities from a DynamoDB table, under a single partition</summary>
        public static async Task QueryAndUpdateAllAsync<TQueryableEntity, TUpdatableEntity>(this IAmazonDynamoDB amazonDynamoDB,
            QueryAndUpdateAllRequest<TQueryableEntity, TUpdatableEntity> request,
            CancellationToken cancellationToken)
            where TQueryableEntity : class, IDynamoDbQueryableEntity<TQueryableEntity>
            where TUpdatableEntity : class, IDynamoDbUpdatableEntity<TUpdatableEntity>
        {
            var queryAllResponse = await amazonDynamoDB.QueryAllAsync(request, cancellationToken);

            foreach (TQueryableEntity entity in queryAllResponse.Entities)
            {
                try // We can't use BatchWriteItem to update items
                {
                    await amazonDynamoDB.UpdateItemAsync(request.UpdateItemRequestFunction(entity), cancellationToken);
                }
                catch (Exception)
                {
                    // Don't throw if exceptions happen
                }
            }
        }

        /// <summary>Write all entities from a DynamoDB table, under a single partition</summary>
        public static async Task QueryAndWriteAllAsync<TQueryableEntity, TWritableEntity>(this IAmazonDynamoDB amazonDynamoDB,
            QueryAndWriteAllRequest<TQueryableEntity, TWritableEntity> request,
            CancellationToken cancellationToken)
            where TQueryableEntity : class, IDynamoDbQueryableEntity<TQueryableEntity>
            where TWritableEntity : class, IDynamoDbBatchWritableEntity<TWritableEntity>
        {
            var queryAllResponse = await amazonDynamoDB.QueryAllAsync(request, cancellationToken);

            try
            {
                await amazonDynamoDB.BatchWriteItemAsync(new BatchWriteItemRequest<TWritableEntity>
                {
                    UserId = request.UserId,
                    RequestEntities = queryAllResponse.Entities.Select(request.WriteRequestFunction).ToList(),
                    ReturnConsumedCapacity = request.ReturnConsumedCapacity,
                    ReturnItemCollectionMetrics = request.ReturnItemCollectionMetrics,
                }, cancellationToken);
            }
            catch (Exception)
            {
                // Don't throw if exceptions happen
            }
        }
    }
}
