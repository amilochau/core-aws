using System.Collections.Generic;

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
    }
}
