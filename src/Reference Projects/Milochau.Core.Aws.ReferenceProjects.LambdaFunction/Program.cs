using Milochau.Core.Aws.Core.Lambda.Events;
using Milochau.Core.Aws.ApiGateway;
using Milochau.Core.Aws.DynamoDB;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunction.DataAccess;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Milochau.Core.Aws.SESv2;
using Milochau.Core.Aws.Lambda;
using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.Lambda.RuntimeSupport.Bootstrap;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction
{
    public class Function
    {

        private static readonly int handlerChoice = 0;
        private static readonly DynamoDbDataAccess dynamoDbDataAccess;
        private static readonly EmailsLambdaDataAccess emailsLambdaDataAccess;
        private static readonly SesDataAccess sesDataAccess;

        static Function()
        {
            var dynamoDBClient = new AmazonDynamoDBClient();
            dynamoDbDataAccess = new DynamoDbDataAccess(dynamoDBClient);
            var lambdaClient = new AmazonLambdaClient();
            emailsLambdaDataAccess = new EmailsLambdaDataAccess(lambdaClient);
            var simpleEmailServiceClient = new AmazonSimpleEmailServiceV2Client();
            sesDataAccess = new SesDataAccess(simpleEmailServiceClient);
        }

        private static async Task Main()
        {
            switch (handlerChoice)
            {
                case 0:
                    await LambdaBootstrap.RunAsync(DoAsync, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyRequest, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyResponse);
                    break;
                case 1:
                    await LambdaBootstrap.RunAsync(DoAsync, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyRequest);
                    break;
                case 2:
                    await LambdaBootstrap.RunAsync(FunctionHandlerScheduler);
                    break;
                case 3:
                    await LambdaBootstrap.RunAsync(FunctionHandlerSns, ApplicationJsonSerializerContext.Default.SNSEvent);
                    break;
                case 4:
                    await LambdaBootstrap.RunAsync(FunctionHandlerDynamoDbStream);
                    break;
            }
        }

        public static Task FunctionHandlerScheduler(Stream requestStream, ILambdaContext context, CancellationToken cancellationToken)
        {
            return DoAsync(new(), context, cancellationToken);
        }

        public static async Task FunctionHandlerSns(SNSEvent request, ILambdaContext context, CancellationToken cancellationToken)
        {
            foreach (var record in request.Records)
            {
                await DoAsync(new(), context, cancellationToken);
            }
        }

        public static Task FunctionHandlerDynamoDbStream(Stream requestStream, ILambdaContext lambdaContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public static async Task<APIGatewayHttpApiV2ProxyResponse> DoAsync(APIGatewayHttpApiV2ProxyRequest request,
            ILambdaContext context,
            CancellationToken cancellationToken)
        {
            if (!request.TryParseAndValidate<FunctionRequest>(new ValidationOptions { AuthenticationRequired = false }, out var proxyResponse, out _))
            {
                return proxyResponse;
            }

            //await sesDataAccess.SendEmailAsync(new(), cancellationToken);
            //await emailsLambdaDataAccess.SendSummaryAsync(cancellationToken);
            await dynamoDbDataAccess.GetTestItemAsync(cancellationToken);

            var response = new FunctionResponse();
            return HttpResponse.Ok(response, ApplicationJsonSerializerContext.Default.FunctionResponse);
        }
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
    [JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
    [JsonSerializable(typeof(SNSEvent))]
    [JsonSerializable(typeof(EmailRequest))]
    [JsonSerializable(typeof(EmailRequestContent))]
    [JsonSerializable(typeof(FunctionResponse))]
    public partial class ApplicationJsonSerializerContext : JsonSerializerContext
    {
    }
}
