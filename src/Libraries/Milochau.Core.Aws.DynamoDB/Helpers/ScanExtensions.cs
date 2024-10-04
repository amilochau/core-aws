using Milochau.Core.Aws.DynamoDB.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB.Helpers
{
    /// <summary>Extension methods for the <c>Scan</c> operation</summary>
    public static class ScanExtensions
    {
        /// <summary>Scan all entities from a DynamoDB table, under all partitions</summary>
        public static async Task<ScanAllResponse<TEntity>> ScanAllAsync<TEntity>(this IAmazonDynamoDB amazonDynamoDB,
            ScanAllRequest<TEntity> request,
            CancellationToken cancellationToken)
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

            var response = new ScanAllResponse<TEntity>();
            do
            {
                var scanResponse = await amazonDynamoDB.ScanAsync(scanRequest, cancellationToken);

                if (scanResponse.Items != null && scanResponse.Entities != null)
                {
                    response.Entities.AddRange(scanResponse.Entities);
                    response.Items.AddRange(scanResponse.Items);
                }

                response.Count += scanResponse.Count ?? 0;
                response.ScannedCount += scanResponse.ScannedCount ?? 0;
                scanRequest.ExclusiveStartKey = scanResponse.LastEvaluatedKey;
            } while (scanRequest.ExclusiveStartKey != null && scanRequest.ExclusiveStartKey.Count > 0);

            return response;
        }

        /// <summary>Update all entities from a DynamoDB table, under all partitions</summary>
        public static async Task ScanAndUpdateAllAsync<TScanableEntity, TUpdatableEntity>(this IAmazonDynamoDB amazonDynamoDB,
            ScanAndUpdateAllRequest<TScanableEntity, TUpdatableEntity> request,
            CancellationToken cancellationToken)
            where TScanableEntity : class, IDynamoDbScanableEntity<TScanableEntity>
            where TUpdatableEntity : class, IDynamoDbUpdatableEntity<TUpdatableEntity>
        {
            var scanAllResponse = await amazonDynamoDB.ScanAllAsync(request, cancellationToken);

            foreach (TScanableEntity entity in scanAllResponse.Entities)
            {
                try // We can't use BatchWriteItem to update items
                {
                    var updateItemResponse = await amazonDynamoDB.UpdateItemAsync(request.UpdateItemRequestFunction(entity), cancellationToken);
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
            var scanAllResponse = await amazonDynamoDB.ScanAllAsync(request, cancellationToken);

            try
            {
                await amazonDynamoDB.BatchWriteItemAsync(new BatchWriteItemRequest<TWritableEntity>
                {
                    UserId = request.UserId,
                    RequestEntities = scanAllResponse.Entities.Select(request.WriteRequestFunction).ToList(),
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
