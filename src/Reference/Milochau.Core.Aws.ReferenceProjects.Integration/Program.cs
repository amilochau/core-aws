﻿using Milochau.Core.Aws.Integration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Text.Json;
using Milochau.Core.Aws.ReferenceProjects.LambdaFunction;
using Milochau.Core.Aws.DynamoDB.Events;
using Milochau.Core.Aws.Core.References.Serialization;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Amazon.Runtime;

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

            app.MapPost("/http", async (HttpContext httpContext, CancellationToken cancellationToken) =>
            {
                var proxyRequest = await ApiGatewayHelpers.BuildProxyRequestAsync(httpContext, new ProxyRequestOptions(), cancellationToken);
                var credentials = new Aws.Integration.AssumeRoleAWSCredentials("arn:aws:iam::339712953809:role/administrator-access");
                var lambdaFunction = new LambdaFunction.Function(credentials);
                var proxyResponse = await lambdaFunction.DoAsync(proxyRequest, new TestLambdaContext(), cancellationToken);
                return ApiGatewayHelpers.BuildResult(proxyResponse);
            })
            .Produces(204)
            .WithTags("Http trigger")
            .WithOpenApi();

            app.MapPost("/dynamodb", async (HttpContext httpContext, CancellationToken cancellationToken) =>
            {
                var proxyRequest = await JsonSerializer.DeserializeAsync(httpContext.Request.Body, new ApplicationJsonSerializerContext2(Options.JsonSerializerOptions).DynamoDBEvent);
                var credentials = new Aws.Integration.AssumeRoleAWSCredentials("arn:aws:iam::339712953809:role/administrator-access");
                var lambdaFunction = new LambdaFunction.Function(credentials);
                await lambdaFunction.FunctionHandlerDynamoDbStream(proxyRequest!, new TestLambdaContext(), cancellationToken);
            })
            .Produces(204)
            .Accepts<DynamoDBEvent>("application/json")
            .WithTags("DynamoDB Stream trigger")
            .WithOpenApi();

            app.Run();
        }
    }
}
