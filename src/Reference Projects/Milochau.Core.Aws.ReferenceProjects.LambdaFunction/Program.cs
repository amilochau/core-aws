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
using Milochau.Core.Aws.DynamoDB.Events;
using System.Collections.Generic;
using System.Linq;
using Milochau.Core.Aws.Core.References.Serialization;
using Milochau.Core.Aws.Cognito;
using Milochau.Core.Aws.Core.Runtime.Credentials;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunction
{
    public class FunctionHandler
    {
        private static readonly int handlerChoice = 0;
        private static readonly Function function = new Function(new EnvironmentVariablesAWSCredentials());

        private static async Task Main()
        {
            switch (handlerChoice)
            {
                case 0:
                    await LambdaBootstrap.RunAsync(function.DoAsync, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyRequest, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyResponse);
                    break;
                case 1:
                    await LambdaBootstrap.RunAsync(function.DoAsync, ApplicationJsonSerializerContext.Default.APIGatewayHttpApiV2ProxyRequest);
                    break;
                case 2:
                    await LambdaBootstrap.RunAsync(function.FunctionHandlerScheduler);
                    break;
                case 3:
                    await LambdaBootstrap.RunAsync(function.FunctionHandlerSns, ApplicationJsonSerializerContext.Default.SNSEvent);
                    break;
                case 4:
                    await LambdaBootstrap.RunAsync(function.FunctionHandlerDynamoDbStream, new ApplicationJsonSerializerContext2(Options.JsonSerializerOptions).DynamoDBEvent, ApplicationJsonSerializerContext2.Default.StreamsEventResponse);
                    break;
            }
        }
    }

    public class Function
    {
        private readonly DynamoDbDataAccess dynamoDbDataAccess;
        private readonly EmailsLambdaDataAccess emailsLambdaDataAccess;
        private readonly SesDataAccess sesDataAccess;
        private readonly CognitoDataAccess cognitoDataAccess;

        public Function(IAWSCredentials credentials)
            : this(dynamoDbDataAccess: new DynamoDbDataAccess(new AmazonDynamoDBClient(credentials)),
                   emailsLambdaDataAccess: new EmailsLambdaDataAccess(new AmazonLambdaClient(credentials)),
                   sesDataAccess: new SesDataAccess(new AmazonSimpleEmailServiceV2Client(credentials)),
                   cognitoDataAccess: new CognitoDataAccess(new AmazonCognitoIdentityProviderClient(credentials)))
        { }

        public Function(DynamoDbDataAccess dynamoDbDataAccess, EmailsLambdaDataAccess emailsLambdaDataAccess, SesDataAccess sesDataAccess, CognitoDataAccess cognitoDataAccess)
        {
            this.dynamoDbDataAccess = dynamoDbDataAccess;
            this.emailsLambdaDataAccess = emailsLambdaDataAccess;
            this.sesDataAccess = sesDataAccess;
            this.cognitoDataAccess = cognitoDataAccess;
        }

        public Task FunctionHandlerScheduler(Stream requestStream, ILambdaContext context, CancellationToken cancellationToken)
        {
            return DoAsync(new(), context, cancellationToken);
        }

        public async Task FunctionHandlerSns(SNSEvent request, ILambdaContext context, CancellationToken cancellationToken)
        {
            foreach (var record in request.Records)
            {
                await DoAsync(new(), context, cancellationToken);
            }
        }

        public Task<StreamsEventResponse> FunctionHandlerDynamoDbStream(DynamoDBEvent request, ILambdaContext lambdaContext, CancellationToken cancellationToken)
        {
            var response = new StreamsEventResponse
            {
                BatchItemFailures = new List<BatchItemFailure>()
            };

            foreach (var record in request.Records.OrderBy(x => x.Dynamodb.ApproximateCreationDateTime))
            {
                try
                {
                    if (record.EventName == OperationType.MODIFY && record.Dynamodb.OldImage != null && record.Dynamodb.NewImage != null)
                    {

                    }
                    else if (record.EventName == OperationType.REMOVE)
                    {

                    }
                }
                catch (System.Exception)
                {
                    response.BatchItemFailures.Add(new BatchItemFailure
                    {
                        ItemIdentifier = record.Dynamodb.SequenceNumber
                    });
                }
            }
            return Task.FromResult(response);
        }

        public async Task<APIGatewayHttpApiV2ProxyResponse> DoAsync(APIGatewayHttpApiV2ProxyRequest request,
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
            //await cognitoDataAccess.LoginAsync(cancellationToken);
            //await cognitoDataAccess.UpdateAttributesAsync(cancellationToken);

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

    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(DynamoDBEvent))]
    [JsonSerializable(typeof(StreamsEventResponse))]
    public partial class ApplicationJsonSerializerContext2 : JsonSerializerContext
    {
    }
}
