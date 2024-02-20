using Milochau.Core.Aws.Core.Lambda.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.ApiGateway
{
    /// <summary>Extensions methods with API Gateway requests</summary>
    public static class HttpRequestExtensions
    {
        /// <summary>Try parse then validate an API Gateway request</summary>
        public static bool TryParseAndValidate<TRequestData>(this APIGatewayHttpApiV2ProxyRequest request, ValidationOptions options,
            [NotNullWhen(false)] out APIGatewayHttpApiV2ProxyResponse? proxyResponse,
            [NotNullWhen(true)] out TRequestData? requestData)
            where TRequestData : IParsableAndValidatable<TRequestData>
        {
            requestData = default;
            proxyResponse = default;
            var modelStateDictionary = new Dictionary<string, Collection<string>>();

            // Authenticate
            if (options.AuthenticationRequired)
            {
                if (!request.TryGetJwtClaims("sub", out var _))
                {
                    proxyResponse = HttpResponse.Unauthorized();
                    return false;
                }
                else if (options.GroupsRequired.Count != 0)
                {
                    if (!request.TryGetJwtClaims("cognito:groups", out var userGroups))
                    {
                        proxyResponse = HttpResponse.Forbidden();
                        return false;
                    }

                    foreach (var group in options.GroupsRequired)
                    {
                        if (!userGroups.Contains(group))
                        {
                            proxyResponse = HttpResponse.Forbidden();
                            return false;
                        }
                    }
                }
            }

            // Parse
            if (!TRequestData.TryParse(request, out requestData))
            {
                proxyResponse = HttpResponse.BadRequest();
                return false;
            }

            // Validate
            requestData.Validate(modelStateDictionary);

            if (modelStateDictionary.Count != 0)
            {
                proxyResponse = HttpResponse.BadRequest(modelStateDictionary);
                return false;
            }

            return true;
        }

        /// <summary>Populate a model state dictionary with errors</summary>
        public static Dictionary<string, Collection<string>> Populate(this Dictionary<string, Collection<string>> modelStateDictionary, string key, string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                if (!modelStateDictionary.ContainsKey(key))
                {
                    modelStateDictionary.Add(key, new Collection<string>());
                }
                modelStateDictionary[key].Add(errorMessage);
            }

            return modelStateDictionary;
        }
    }

    /// <summary>JSON serializer context used to serialize validation problem details</summary>
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(ValidationProblemDetails))]
    public partial class HelpersJsonSerializerContext : JsonSerializerContext
    {
    }
}
