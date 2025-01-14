using Milochau.Core.Aws.Integration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunction;
using System;
using Milochau.Core.Aws.DynamoDB.Events;

var options = new IntegrationWebApplicationOptions
{
    Args = args,
    CorsOrigins = ["http://localhost:3000", "http://localhost:4173"]
};

var builder = IntegrationWebApplication.CreateBuilder(options);

var app = builder.Build();

app.UseIntegrationMiddlewares(options);

app.MapPost("/dynamodb", async (DynamoDBEvent proxyRequest, CancellationToken cancellationToken) =>
{
    var credentials = new AssumeRoleAWSCredentials(Environment.GetEnvironmentVariable("AWS_ROLE_ARN")!);
    var lambdaFunction = new Function(credentials);
    await Function.FunctionHandlerDynamoDbStream(proxyRequest!, new TestLambdaContext(), cancellationToken);
})
.Produces(200)
.WithTags("DynamoDB Stream trigger");

app.Run();