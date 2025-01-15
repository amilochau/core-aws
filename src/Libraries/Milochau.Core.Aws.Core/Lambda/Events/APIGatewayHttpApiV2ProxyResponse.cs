using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.Lambda.Events
{
    /// <summary>
    /// The response object for Lambda functions handling request from API Gateway HTTP API v2 proxy format
    /// https://docs.aws.amazon.com/apigateway/latest/developerguide/http-api-develop-integrations-lambda.html
    /// </summary>
    public class APIGatewayHttpApiV2ProxyResponse
    {
        /// <summary>The HTTP status code for the request</summary>
        public int StatusCode { get; set; }

        /// <summary>The Http headers returned in the response. Multiple header values set for the the same header should be separate by a comma.</summary>
        public Dictionary<string, string?> Headers { get; set; } = [];

        /// <summary>The cookies returned in the response</summary>
        public string?[]? Cookies { get; set; }

        /// <summary>The response body</summary>
        public string? Body { get; set; }

        /// <summary>Flag indicating whether the body should be treated as a base64-encoded string</summary>
        public bool? IsBase64Encoded { get; set; }

        public void SetHeaderValues(string headerName, IEnumerable<string?> values, bool append)
        {
            if (Headers.TryGetValue(headerName, out string? value) && append)
            {
                Headers[headerName] = value + "," + string.Join(",", values);
            }
            else
            {
                Headers[headerName] = string.Join(",", values);
            }
        }
    }
}
