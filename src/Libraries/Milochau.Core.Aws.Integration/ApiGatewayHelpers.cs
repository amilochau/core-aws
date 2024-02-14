using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using System.Text;
using Milochau.Core.Aws.Core.Lambda.Events;

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

            if (!options.AnonymousRequest)
            {
                proxyRequest.RequestContext.Authorizer = new APIGatewayHttpApiV2ProxyRequest.AuthorizerDescription
                {
                    Jwt = new APIGatewayHttpApiV2ProxyRequest.AuthorizerDescription.JwtDescription
                    {
                        Claims = new Dictionary<string, string>
                        {
                            { "sub", options.UserSub },
                            { "name", options.UserName },
                            { "email", options.UserEmail },
                            { "custom:user_id", options.UserId },
                        },
                    },
                };
            }

            if (options.PathParameters.Count != 0)
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
