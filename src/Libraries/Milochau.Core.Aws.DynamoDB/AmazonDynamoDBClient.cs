using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Milochau.Core.Aws.Core.Runtime.Internal;
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
        public Task<BatchWriteItemResponse> BatchWriteItemAsync(BatchWriteItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = BatchWriteItemRequestMarshaller.Instance,
                ResponseUnmarshaller = BatchWriteItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "BatchWriteItem",
            };

            return InvokeAsync<BatchWriteItemResponse>(request, options, cancellationToken);
        }

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
        public Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = DeleteItemRequestMarshaller.Instance,
                ResponseUnmarshaller = DeleteItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "DeleteItem",
            };

            return InvokeAsync<DeleteItemResponse>(request, options, cancellationToken);
        }

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
        public Task<GetItemResponse> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = GetItemRequestMarshaller.Instance,
                ResponseUnmarshaller = GetItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "GetItem",
            };

            return InvokeAsync<GetItemResponse>(request, options, cancellationToken);
        }

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
        public Task<PutItemResponse> PutItemAsync(PutItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = PutItemRequestMarshaller.Instance,
                ResponseUnmarshaller = PutItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "PutItem",
            };

            return InvokeAsync<PutItemResponse>(request, options, cancellationToken);
        }

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
        public Task<QueryResponse> QueryAsync(QueryRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = QueryRequestMarshaller.Instance,
                ResponseUnmarshaller = QueryResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "Query",
            };

            return InvokeAsync<QueryResponse>(request, options, cancellationToken);
        }

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
        public Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions
            {
                HttpRequestMessageMarshaller = UpdateItemRequestMarshaller.Instance,
                ResponseUnmarshaller = UpdateItemResponseUnmarshaller.Instance,
                MonitoringOriginalRequestName = "UpdateItem",
            };

            return InvokeAsync<UpdateItemResponse>(request, options, cancellationToken);
        }

        #endregion
    }
}
