using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
//using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Milochau.Core.Aws.ApiGateway;
using Milochau.Core.Aws.ApiGateway.APIGatewayEvents;
using Milochau.Core.Aws.DynamoDB.DynamoDBv2;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction
{
    public class Function
    {
        public static readonly int handlerChoice = 1;
        private static async Task Main()
        {
            //AWSSDKHandler.RegisterXRayForAllServices();

            switch (handlerChoice)
            {
                case 0:
                    await LambdaBootstrapBuilder.Create(FunctionHandlerHttp).Build().RunAsync();
                    break;
                case 1:
                    await LambdaBootstrapBuilder.Create(FunctionHandlerAsync).Build().RunAsync();
                    break;
            }
        }

        public static async Task<Stream> FunctionHandlerHttp(Stream requestStream, ILambdaContext context)
        {
            APIGatewayHttpApiV2ProxyResponse response;
            try
            {
                var cancellationToken = CancellationToken.None;

                var utf8Json = (requestStream as MemoryStream)!.ToArray();
                var request = JsonSerializer.Deserialize(utf8Json, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyRequest)!;

                using var dynamoDBClient = new AmazonDynamoDBClient();
                var dynamoDbDataAccess = new DynamoDbDataAccess(dynamoDBClient);
                using var lambdaClient = new AmazonLambdaClient();
                var emailsLambdaDataAccess = new EmailsLambdaDataAccess(lambdaClient);

                response = await DoAsync(request, context, dynamoDbDataAccess, emailsLambdaDataAccess, cancellationToken);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error during test {ex.Message} {ex.StackTrace}");
                response = HttpResponse.InternalServerError();
            }

            var responseStream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(responseStream))
            {
                JsonSerializer.Serialize(writer, response, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyResponse);
            }
            responseStream.Position = 0;
            return responseStream;
        }

        public static async Task FunctionHandlerAsync(Stream requestStream, ILambdaContext context)
        {
            var cancellationToken = CancellationToken.None;

            var utf8Json = (requestStream as MemoryStream)!.ToArray();
            // Note: the following line should not deserialize as APIGatewayHttpApiV2ProxyRequest - but here we do that to help tests
            var request = JsonSerializer.Deserialize(utf8Json, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyRequest)!;

            using var dynamoDBClient = new AmazonDynamoDBClient();
            var dynamoDbDataAccess = new DynamoDbDataAccess(dynamoDBClient);
            using var lambdaClient = new AmazonLambdaClient();
            var emailsLambdaDataAccess = new EmailsLambdaDataAccess(lambdaClient);

            await DoAsync(request, context, dynamoDbDataAccess, emailsLambdaDataAccess, cancellationToken);
        }

        public static async Task FunctionHandlerScheduler(Stream request, ILambdaContext context)
        {
            try
            {
                var cancellationToken = CancellationToken.None;

                using var dynamoDBClient = new AmazonDynamoDBClient();
                var dynamoDbDataAccess = new DynamoDbDataAccess(dynamoDBClient);
                using var lambdaClient = new AmazonLambdaClient();
                var emailsLambdaDataAccess = new EmailsLambdaDataAccess(lambdaClient);

                await DoAsync(new(), context, dynamoDbDataAccess, emailsLambdaDataAccess, cancellationToken);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error during test {ex.Message} {ex.StackTrace}");
                throw;
            }
        }


        public static async Task<APIGatewayHttpApiV2ProxyResponse> DoAsync(APIGatewayHttpApiV2ProxyRequest request,
            ILambdaContext context,
            IDynamoDbDataAccess dynamoDbDataAccess,
            IEmailsLambdaDataAccess emailsLambdaDataAccess,
            CancellationToken cancellationToken)
        {
            if (!request.TryParseAndValidate<LambdaFunctionRequest>(new ValidationOptions { AuthenticationRequired = false }, out var proxyResponse, out var requestData))
            {
                return proxyResponse;
            }

            await dynamoDbDataAccess.GetMessageAsync(cancellationToken);

            await Task.CompletedTask;

            return HttpResponse.NoContent();
        }
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
    [JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
    [JsonSerializable(typeof(EmailRequest))]
    [JsonSerializable(typeof(EmailRequestContent))]
    public partial class ApplicationJsonSerializerContext : JsonSerializerContext
    {
    }
}
