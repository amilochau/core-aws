using Microsoft.Extensions.Configuration;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils
{
    /// <summary>
    /// Utility class to read <see cref="IConfiguration"/> object.
    /// </summary>
    public static class XRayConfiguration
    {
        private const string DEFAULT_CONFIG_SECTION = "XRay";
        private const string PluginSettingKey = "AWSXRayPlugins";
        private const string SamplingRuleManifestKey = "SamplingRuleManifest";
        private const string AWSServiceHandlerManifestKey = "AWSServiceHandlerManifest";
        private const string DisableXRayTracingKey = "DisableXRayTracing";
        private const string UseRuntimeErrorsKey = "UseRuntimeErrors";
        private const string CollectSqlQueries = "CollectSqlQueries";

        /// <summary>
        /// Reads configuration from <see cref="IConfiguration"/> object for X-Ray.
        /// </summary>
        /// <param name="config">Instance of <see cref="IConfiguration"/>.</param>
        /// <returns>Instance of <see cref="XRayOptions"/>.</returns>
        public static XRayOptions GetXRayOptions(this IConfiguration? config)
        {
            return GetXRayOptions(config, DEFAULT_CONFIG_SECTION);
        }

        private static XRayOptions GetXRayOptions(IConfiguration? config, string configSection)
        {
            var options = new XRayOptions();

            IConfiguration section;
            if (Equals(config, null))
                return options;

            if (string.IsNullOrEmpty(configSection))
                section = config;
            else
                section = config.GetSection(configSection);

            if (section == null)
                return options;

            options.PluginSetting = GetSetting(PluginSettingKey, section);
            options.SamplingRuleManifest = GetSetting(SamplingRuleManifestKey, section);
            options.AwsServiceHandlerManifest = GetSetting(AWSServiceHandlerManifestKey, section);
            options.IsXRayTracingDisabled = GetSettingBool(DisableXRayTracingKey, section);
            options.UseRuntimeErrors = GetSettingBool(UseRuntimeErrorsKey, section, defaultValue: false);
            options.CollectSqlQueries = GetSettingBool(CollectSqlQueries, section, defaultValue: false);
            return options;
        }

        private static string? GetSetting(string key, IConfiguration section)
        {
            if (!string.IsNullOrEmpty(section[key]))
            {
                return section[key];
            }
            else
                return null;
        }

        private static bool GetSettingBool(string key, IConfiguration section, bool defaultValue = false)
        {
            string? value = GetSetting(key, section);
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
