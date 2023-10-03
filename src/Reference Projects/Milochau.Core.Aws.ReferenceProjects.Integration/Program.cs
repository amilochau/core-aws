using Amazon.Lambda.TestUtilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Milochau.Core.Aws.DynamoDB.DynamoDBv2;
using Milochau.Core.Aws.Integration;
using System.Threading;

namespace Milochau.Core.Aws.ReferenceProjects.Integration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors();

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:3000");
            });
            app.MapGet("/", () =>
            {
                return Results.Redirect("/swagger");
            }).ExcludeFromDescription();

            app.MapGet("/lambda-function", async (HttpContext httpContext, CancellationToken cancellationToken) =>
            {
                var proxyRequest = await ApiGatewayHelpers.BuildProxyRequestAsync(httpContext, new ProxyRequestOptions(), cancellationToken);
                var dynamoDbDataAccess = new LambdaFunction.DataAccess.DynamoDbDataAccess(new AmazonDynamoDBClient());
                var proxyResponse = await LambdaFunction.Function.DoAsync(proxyRequest, new TestLambdaContext(), dynamoDbDataAccess, cancellationToken);
                return ApiGatewayHelpers.BuildEmptyResult(proxyResponse);
            })
            .Produces(204)
            .WithTags("Lambda Function")
            .WithSummary("Call the Lambda Function")
            .WithOpenApi();

            app.Run();
        }
    }
}
