using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>Helpers with API Gateway</summary>
    public static class ApiGatewayHelpers
    {
        /// <summary>Build a proxy request from an HTTP context</summary>
        public static async Task<APIGatewayHttpApiV2ProxyRequest> BuildProxyRequestAsync(HttpContext httpContext, ProxyRequestOptions options, CancellationToken cancellationToken)
        {
            var proxyRequest = new APIGatewayHttpApiV2ProxyRequest
            {
                RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext
                {
                    Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription
                    {
                        Method = httpContext.Request.Method,
                        Path = httpContext.Request.Path,
                        Protocol = httpContext.Request.Protocol,
                    },
                },
                Body = await new StreamReader(httpContext.Request.Body).ReadToEndAsync(cancellationToken),
            };

            if (options.UserId != null)
            {
                proxyRequest.RequestContext.Authorizer = new APIGatewayHttpApiV2ProxyRequest.AuthorizerDescription
                {
                    Jwt = new APIGatewayHttpApiV2ProxyRequest.AuthorizerDescription.JwtDescription
                    {
                        Claims = new Dictionary<string, string>
                        {
                            { "sub", options.UserId },
                            { "name", options.UserName },
                            { "email", options.UserEmail },
                        },
                    },
                };
            }

            if (options.PathParameters.Any())
            {
                proxyRequest.PathParameters = options.PathParameters;
            }

            // Query string parameters
            proxyRequest.QueryStringParameters = new Dictionary<string, string>();
            foreach (var queryParameter in httpContext.Request.Query)
            {
                proxyRequest.QueryStringParameters.Add(queryParameter.Key, queryParameter.Value.ToString());
            }

            return proxyRequest;
        }

        /// <summary>Build a JSON HTTP result from a proxy response</summary>
        public static ContentHttpResult BuildResult(APIGatewayHttpApiV2ProxyResponse proxyResponse)
        {
            if (proxyResponse.Body == null)
            {
                return TypedResults.Text(string.Empty, statusCode: proxyResponse.StatusCode);
            }
            return TypedResults.Text(proxyResponse.Body, "application/json", Encoding.UTF8, proxyResponse.StatusCode);
        }

        /// <summary>Build an empty JSON HTTP result from a proxy response</summary>
        public static ContentHttpResult BuildEmptyResult(APIGatewayHttpApiV2ProxyResponse proxyResponse)
        {
            return TypedResults.Text(proxyResponse.Body, statusCode: proxyResponse.StatusCode);
        }
    }
}
