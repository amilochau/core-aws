using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using Milochau.Core.Aws.Core.Lambda.Events;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>HTTP response</summary>
    public static class HttpResponse
    {
        /// <summary>Ok - 200</summary>
        public static APIGatewayHttpApiV2ProxyResponse Ok<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(value, jsonTypeInfo),
                Headers = new Dictionary<string, string?>
                {
                    { "Content-Type", "application/json" },
                },
            };
        }

        /// <summary>No Content - 204</summary>
        public static APIGatewayHttpApiV2ProxyResponse NoContent()
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 204,
            };
        }

        /// <summary>Bad Request - 400</summary>
        public static APIGatewayHttpApiV2ProxyResponse BadRequest()
        {
            return BadRequest([]);
        }

        /// <summary>Bad Request - 400</summary>
        public static APIGatewayHttpApiV2ProxyResponse BadRequest(Dictionary<string, Collection<string>> modelStateDictionary)
        {
            var problemDetails = new ValidationProblemDetails(modelStateDictionary.AsReadOnly()) { Status = 400 };
            return BadRequest(problemDetails);
        }

        /// <summary>Bad Request - 400</summary>
        public static APIGatewayHttpApiV2ProxyResponse BadRequest(ValidationProblemDetails problemDetails)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 400,
                Body = JsonSerializer.Serialize(problemDetails, HelpersJsonSerializerContext.Default.ValidationProblemDetails),
                Headers = new Dictionary<string, string?>
                {
                    { "Content-Type", "application/json" },
                },
            };
        }

        /// <summary>Unauthorized - 400 (with an internal 401 status code)</summary>
        public static APIGatewayHttpApiV2ProxyResponse Unauthorized()
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            var problemDetails = new ValidationProblemDetails(modelStateDictionary.AsReadOnly()) { Status = 401 };
            return BadRequest(problemDetails);
        }

        /// <summary>Forbidden - 400 (with an internal 403 status code)</summary>
        public static APIGatewayHttpApiV2ProxyResponse Forbidden()
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            var problemDetails = new ValidationProblemDetails(modelStateDictionary.AsReadOnly()) { Status = 403 };
            return BadRequest(problemDetails);
        }

        /// <summary>Not Found - 400 (with an internal 404 status code)</summary>
        public static APIGatewayHttpApiV2ProxyResponse NotFound()
        {
            var modelStateDictionary = new Dictionary<string, Collection<string>>();
            var problemDetails = new ValidationProblemDetails(modelStateDictionary.AsReadOnly()) { Status = 404 };
            return BadRequest(problemDetails);
        }

        /// <summary>Internal Server Error - 500</summary>
        public static APIGatewayHttpApiV2ProxyResponse InternalServerError()
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 500,
            };
        }
    }
}
