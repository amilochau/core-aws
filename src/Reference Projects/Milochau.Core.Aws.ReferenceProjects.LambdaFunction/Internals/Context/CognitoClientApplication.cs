using System.Text.Json;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context
{
    internal class CognitoClientApplication : IClientApplication
    {
        public string? AppPackageName { get; internal set; }

        public string? AppTitle { get; internal set; }

        public string? AppVersionCode { get; internal set; }

        public string? AppVersionName { get; internal set; }

        public string? InstallationId { get; internal set; }

        internal static CognitoClientApplication FromJsonData(JsonElement jsonData)
        {
            var result = new CognitoClientApplication();

            if (jsonData.TryGetProperty("app_package_name", out var nameElement))
                result.AppPackageName = nameElement.GetString();
            if (jsonData.TryGetProperty("app_title", out var tileElement))
                result.AppTitle = tileElement.GetString();
            if (jsonData.TryGetProperty("app_version_code", out var codeElement))
                result.AppVersionCode = codeElement.GetString();
            if (jsonData.TryGetProperty("app_version_name", out var versionNameElement))
                result.AppVersionName = versionNameElement.GetString();
            if (jsonData.TryGetProperty("installation_id", out var installElement))
                result.InstallationId = installElement.GetString();

            return result;
        }
    }
}
