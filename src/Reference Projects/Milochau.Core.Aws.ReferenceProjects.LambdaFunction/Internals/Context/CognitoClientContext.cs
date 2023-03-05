using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context
{
    internal class CognitoClientContext : IClientContext
    {
        public IDictionary<string, string>? Environment { get; internal set; }

        public IClientApplication? Client { get; internal set; }

        public IDictionary<string, string>? Custom { get; internal set; }

        internal static CognitoClientContext FromJson(string? json)
        {
            var result = new CognitoClientContext();

            if (!string.IsNullOrWhiteSpace(json))
            {
                var jsonData = JsonDocument.Parse(json).RootElement;

                if (jsonData.TryGetProperty("client", out var clientElement))
                    result.Client = CognitoClientApplication.FromJsonData(clientElement);
                if (jsonData.TryGetProperty("custom", out var customElement))
                    result.Custom = GetDictionaryFromJsonData(customElement);
                if (jsonData.TryGetProperty("env", out var envElement))
                    result.Environment = GetDictionaryFromJsonData(envElement);

                return result;
            }

            return result;
        }

        private static IDictionary<string, string> GetDictionaryFromJsonData(JsonElement jsonData)
        {
            return jsonData.EnumerateObject().ToDictionary(properties => properties.Name, properties => properties.Value.ToString());
        }
    }
}
