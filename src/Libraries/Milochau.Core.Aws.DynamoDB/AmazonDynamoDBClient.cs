using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model;
using Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB
{
    /// <summary>
    /// Implementation for accessing DynamoDB
    /// </summary>
    public partial class AmazonDynamoDBClient : AmazonServiceClient, IAmazonDynamoDB
    {
        #region Constructors

        /// <summary>
        /// Constructs AmazonDynamoDBClient
        /// </summary>
        public AmazonDynamoDBClient(IAWSCredentials credentials)
            : base(credentials, new ClientConfig
            {
                AuthenticationServiceName = "dynamodb",
                MonitoringServiceName = "DynamoDBv2"
            })
        { }

        #endregion

        #region  BatchWriteItem

        /// <inheritdoc/>
        public async Task<List<BatchWriteItemResponse<TEntity>>> BatchWriteItemAsync<TEntity>(BatchWriteItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbBatchWritableEntity<TEntity>
        {
            var response = new List<BatchWriteItemResponse<TEntity>>();

            foreach (var singleRequest in request.Build())
            {
                try
                {
                    var singleResponse = await BatchWriteItemAsync(singleRequest, cancellationToken);
                    response.Add(new BatchWriteItemResponse<TEntity>(singleResponse));
                }
                catch (System.Exception)
                {
                    // Continue
                }
            }

            return response;
        }

        /// <inheritdoc/>
        public Task<BatchWriteItemResponse> BatchWriteItemAsync(BatchWriteItemRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new BatchWriteItemInvokeOptions(), cancellationToken);

        #endregion
        #region  DeleteItem

        /// <inheritdoc/>
        public async Task<DeleteItemResponse<TEntity>> DeleteItemAsync<TEntity>(DeleteItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbDeletableEntity<TEntity>
        {
            var response = await DeleteItemAsync(request.Build(), cancellationToken);
            return new DeleteItemResponse<TEntity>(response);
        }

        /// <inheritdoc/>
        public Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new DeleteItemInvokeOptions(), cancellationToken);

        #endregion
        #region  GetItem

        /// <inheritdoc/>
        public async Task<GetItemResponse<TEntity>> GetItemAsync<TEntity>(GetItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity: class, IDynamoDbGettableEntity<TEntity>
        {
            var response = await GetItemAsync(request.Build(), cancellationToken);
            return new GetItemResponse<TEntity>(response);
        }

        /// <inheritdoc/>
        public Task<GetItemResponse> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new GetItemInvokeOptions(), cancellationToken);

        #endregion
        #region  PutItem

        /// <inheritdoc/>
        public async Task<PutItemResponse<TEntity>> PutItemAsync<TEntity>(PutItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbPutableEntity<TEntity>
        {
            var response = await PutItemAsync(request.Build(), cancellationToken);
            return new PutItemResponse<TEntity>(response);
        }

        /// <inheritdoc/>
        public Task<PutItemResponse> PutItemAsync(PutItemRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new PutItemInvokeOptions(), cancellationToken);

        #endregion
        #region  UpdateItem

        /// <inheritdoc/>
        public async Task<UpdateItemResponse<TEntity>> UpdateItemAsync<TEntity>(UpdateItemRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbUpdatableEntity<TEntity>
        {
            var response = await UpdateItemAsync(request.Build(), cancellationToken);
            return new UpdateItemResponse<TEntity>(response);
        }

        /// <inheritdoc/>
        public Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new UpdateItemInvokeOptions(), cancellationToken);

        #endregion
        #region  Query

        /// <inheritdoc/>
        public async Task<QueryResponse<TEntity>> QueryAsync<TEntity>(QueryRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbQueryableEntity<TEntity>
        {
            var response = await QueryAsync(request.Build(), cancellationToken);

            return new QueryResponse<TEntity>(response);
        }

        /// <inheritdoc/>
        public Task<QueryResponse> QueryAsync(QueryRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new QueryInvokeOptions(), cancellationToken);

        #endregion
        #region Scan

        /// <inheritdoc/>
        public async Task<ScanResponse<TEntity>> ScanAsync<TEntity>(ScanRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class, IDynamoDbScanableEntity<TEntity>
        {
            var response = await ScanAsync(request.Build(), cancellationToken);

            return new ScanResponse<TEntity>(response);
        }

        /// <inheritdoc/>
        public Task<ScanResponse> ScanAsync(ScanRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new ScanInvokeOptions(), cancellationToken);

        #endregion
    }
}
