using Amazon.DynamoDBv2;
using Milochau.Core.Aws.ApiGateway;
using Milochau.Core.Aws.ApiGateway.APIGatewayEvents;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction
{
    public class Function
    {
        private static async Task Main()
        {
            var handlerWrapper = HandlerWrapper.GetHandlerWrapper(FunctionHandler,
                deserializerInput: stream => JsonSerializer.Deserialize(stream, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyRequest),
                serializeOutput: (output, stream) => JsonSerializer.Serialize(stream, output, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyResponse));

            using var httpClient = new HttpClient(new SocketsHttpHandler()) { Timeout = TimeSpan.FromHours(12) };

            var lambdaBootstrap = new LambdaBootstrap(httpClient, handlerWrapper.Handler);
            await lambdaBootstrap.RunAsync();
        }

        public static async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest? request, ILambdaContext context)
        {
            try
            {
                var cancellationToken = CancellationToken.None;
                if (request == null)
                {
                    throw new Exception("Request can not be deserialized");
                }

                using var dynamoDBClient = new AmazonDynamoDBClient();
                var dynamoDbDataAccess = new DynamoDbDataAccess(dynamoDBClient);

                return await DoAsync(request, context, dynamoDbDataAccess, cancellationToken);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error during test {ex.Message} {ex.StackTrace}");
                return HttpResponse.InternalServerError();
            }
        }

        public static async Task<APIGatewayHttpApiV2ProxyResponse> DoAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context, IDynamoDbDataAccess dynamoDbDataAccess, CancellationToken cancellationToken)
        {
            if (!request.TryParseAndValidate<LambdaFunctionRequest>(new ValidationOptions { AuthenticationRequired = false }, out var proxyResponse, out var requestData))
            {
                return proxyResponse;
            }

            await Task.CompletedTask;

            return HttpResponse.NoContent();
        }
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
    [JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
    [JsonSerializable(typeof(StatusResponse))]
    [JsonSerializable(typeof(Exception))]
    public partial class ApplicationJsonSerializerContext : JsonSerializerContext
    {
    }
}
