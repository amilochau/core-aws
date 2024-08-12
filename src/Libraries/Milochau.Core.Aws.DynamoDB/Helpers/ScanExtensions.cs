using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Extension methods for the <c>Scan</c> operation</summary>
    public static class ScanExtensions
    {
        /// <summary>Scan all entities from a DynamoDB table, under all partitions</summary>
        public static async IAsyncEnumerable<TEntity> ScanAllAsync<TEntity>(this IAmazonDynamoDB amazonDynamoDB,
            ScanAllRequest<TEntity> request,
            [EnumeratorCancellation] CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbScanableEntity<TEntity>
        {
            var scanRequest = new ScanRequest<TEntity>
            {
                UserId = request.UserId,
                ReturnConsumedCapacity = request.ReturnConsumedCapacity,
                Filters = request.Filters,
                ConsistentRead = request.ConsistentRead,
                Segment = request.Segment,
                TotalSegments = request.TotalSegments,
            };

            do
            {
                var response = await amazonDynamoDB.ScanAsync(scanRequest, cancellationToken);
                if (response.Entities == null || response.Entities.Count == 0)
                {
                    break;
                }
                foreach (var entity in response.Entities)
                {
                    yield return entity;
                }
                scanRequest.ExclusiveStartKey = response.LastEvaluatedKey;
            } while (scanRequest.ExclusiveStartKey != null);
        }

        /// <summary>Update all entities from a DynamoDB table, under all partitions</summary>
        public static async Task ScanAndUpdateAllAsync<TScanableEntity, TUpdatableEntity>(this IAmazonDynamoDB amazonDynamoDB,
            ScanAndUpdateAllRequest<TScanableEntity, TUpdatableEntity> request,
            CancellationToken cancellationToken)
            where TScanableEntity : class, IDynamoDbScanableEntity<TScanableEntity>
            where TUpdatableEntity : class, IDynamoDbUpdatableEntity<TUpdatableEntity>
        {
            var entities = amazonDynamoDB.ScanAllAsync(request, cancellationToken);

            await foreach (TScanableEntity entity in entities)
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

        /// <summary>Write all entities from a DynamoDB table, under all partitions</summary>
        public static async Task ScanAndWriteAllAsync<TScanableEntity, TWritableEntity>(this IAmazonDynamoDB amazonDynamoDB,
            ScanAndWriteAllRequest<TScanableEntity, TWritableEntity> request,
            CancellationToken cancellationToken)
            where TScanableEntity : class, IDynamoDbScanableEntity<TScanableEntity>
            where TWritableEntity : class, IDynamoDbBatchWritableEntity<TWritableEntity>
        {
            var entities = amazonDynamoDB.ScanAllAsync(request, cancellationToken);

            List<WriteRequest<TWritableEntity>> writeRequests = [];
            await foreach (TScanableEntity entity in entities)
            {
                writeRequests.Add(request.WriteRequestFunction(entity));
            }

            try
            {
                await amazonDynamoDB.BatchWriteItemAsync(new BatchWriteItemRequest<TWritableEntity>
                {
                    UserId = request.UserId,
                    RequestEntities = writeRequests,
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
