using Milochau.Core.Aws.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Milochau.Core.Aws.Core.Lambda.Events
{
    /// <summary>
    /// For request using using API Gateway HTTP API version 2 payload proxy format
    /// https://docs.aws.amazon.com/apigateway/latest/developerguide/http-api-develop-integrations-lambda.html
    /// </summary>
    public class APIGatewayHttpApiV2ProxyRequest
    {
        /// <summary>
        /// Headers sent with the request. Multiple values for the same header will be separated by a comma.
        /// </summary>
        public IDictionary<string, string>? Headers { get; set; }

        /// <summary>
        /// Query string parameters sent with the request. Multiple values for the same parameter will be separated by a comma.
        /// </summary>
        public IDictionary<string, string>? QueryStringParameters { get; set; }

        /// <summary>
        /// The request context for the request
        /// </summary>
        public required ProxyRequestContext RequestContext { get; set; }

        /// <summary>
        /// The HTTP request body.
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// Path parameters sent with the request.
        /// </summary>
        public IDictionary<string, string>? PathParameters { get; set; }

        /// <summary>
        /// The ProxyRequestContext contains the information to identify the AWS account and resources invoking the 
        /// Lambda function.
        /// </summary>
        public class ProxyRequestContext
        {
            /// <summary>
            /// The domain name.
            /// </summary>
            public string? DomainName { get; set; }

            /// <summary>
            /// Information about the current requesters authorization including claims and scopes.
            /// </summary>
            public AuthorizerDescription? Authorizer { get; set; }

            /// <summary>
            /// Information about the HTTP request like the method and path.
            /// </summary>
            public HttpDescription? Http { get; set; }
        }

        /// <summary>
        /// Information about the HTTP elements for the request.
        /// </summary>
        public class HttpDescription
        {
            /// <summary>
            /// The HTTP method like POST or GET.
            /// </summary>
            public string? Method { get; set; }

            /// <summary>
            /// The path of the request.
            /// </summary>
            public string? Path { get; set; }

            /// <summary>
            /// The protocal used to make the rquest
            /// </summary>
            public string? Protocol { get; set; }

            /// <summary>
            /// The source ip for the request.
            /// </summary>
            public string? SourceIp { get; set; }

            /// <summary>
            /// The user agent for the request.
            /// </summary>
            public string? UserAgent { get; set; }
        }

        /// <summary>
        /// Information about the current requesters authorization.
        /// </summary>
        public class AuthorizerDescription
        {
            /// <summary>
            /// The JWT description including claims and scopes.
            /// </summary>
            public JwtDescription? Jwt { get; set; }

            /// <summary>
            /// Describes the information in the JWT token
            /// </summary>
            public class JwtDescription
            {
                /// <summary>
                /// Map of the claims for the requester.
                /// </summary>
                public IDictionary<string, string>? Claims { get; set; }
            }
        }


        /// <summary>Try get JWT claims</summary>
        public bool TryGetJwtClaims(string key, [NotNullWhen(true)] out string? value)
        {
            value = null;
            return RequestContext.Authorizer?.Jwt?.Claims?.TryGetValue(key, out value) ?? false;
        }

        /// <summary>Try get Identity User</summary>
        public bool TryGetIdentityUser([NotNullWhen(true)] out IdentityUser? user)
        {
            user = null;

            if (!TryGetJwtClaims("sub", out var userSub)
                || !TryGetJwtClaims("email", out var userEmail)
                || !TryGetJwtClaims("name", out var userName)
                || !TryGetJwtClaims("custom:user_id", out var rawUserId) || !Guid.TryParse(rawUserId, out var userId))
            {
                return false;
            }

            user = new IdentityUser(userSub, userName, userEmail, userId);
            return true;
        }

        /// <summary>Try get the value of a path parameter</summary>
        public bool TryGetPathParameter(string key, [NotNullWhen(true)] out string? value)
        {
            value = null;
            return PathParameters?.TryGetValue(key, out value) ?? false;
        }

        /// <summary>Try get the value of a query string parameter</summary>
        public bool TryGetQueryStringParameter(string key, [NotNullWhen(true)] out string? value)
        {
            value = null;
            return QueryStringParameters?.TryGetValue(key, out value) ?? false;
        }

        /// <summary>Try get the value of a header</summary>
        public bool TryGetHeader(string key, [NotNullWhen(true)] out string? value)
        {
            value = null;
            return Headers?.TryGetValue(key, out value) ?? false;
        }

        /// <summary>Try deserialize the body</summary>
        public bool TryDeserializeBody<TBody>([NotNullWhen(true)] out TBody? value, JsonTypeInfo<TBody> jsonTypeInfo)
        {
            value = default;
            if (Body == null)
            {
                return false;
            }

            value = JsonSerializer.Deserialize(Body, jsonTypeInfo);
            return value != null;
        }
    }
}
