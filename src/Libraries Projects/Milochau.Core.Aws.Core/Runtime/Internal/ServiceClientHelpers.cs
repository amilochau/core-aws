using System;
using System.Linq;
using System.Reflection;
using Amazon.Util.Internal;

namespace Amazon.Runtime.Internal
{
    public static class ServiceClientHelpers
    {
        public const string S3_ASSEMBLY_NAME = "AWSSDK.S3";
        public const string S3_SERVICE_CLASS_NAME = "Amazon.S3.AmazonS3Client";

        public const string SSO_ASSEMBLY_NAME = "AWSSDK.SSO";
        public const string SSO_SERVICE_CLASS_NAME = "Amazon.SSO.AmazonSSOClient";
        public const string SSO_SERVICE_CONFIG_NAME = "Amazon.SSO.AmazonSSOConfig";

        public const string SSO_OIDC_ASSEMBLY_NAME = "AWSSDK.SSOOIDC";
        public const string SSO_OIDC_SERVICE_CLASS_NAME = "Amazon.SSOOIDC.AmazonSSOOIDCClient";
        public const string SSO_OIDC_SERVICE_CONFIG_NAME = "Amazon.SSOOIDC.AmazonSSOOIDCConfig";

        public const string STS_ASSEMBLY_NAME = "AWSSDK.SecurityToken";
        public const string STS_SERVICE_CLASS_NAME = "Amazon.SecurityToken.AmazonSecurityTokenServiceClient";
        public const string STS_SERVICE_CONFIG_NAME = "Amazon.SecurityToken.AmazonSecurityTokenServiceConfig";

        public const string KMS_ASSEMBLY_NAME = "AWSSDK.KeyManagementService";
        public const string KMS_SERVICE_CLASS_NAME = "Amazon.KeyManagementService.AmazonKeyManagementServiceClient";

        public static TClient CreateServiceFromAssembly<TClient>(string assemblyName, string serviceClientClassName, 
            AWSCredentials credentials, RegionEndpoint region)
            where TClient : class
        {
            var serviceClientTypeInfo = LoadServiceClientType(assemblyName, serviceClientClassName);

            var constructor = serviceClientTypeInfo.GetConstructor(new ITypeInfo[]
                {
                    TypeFactory.GetTypeInfo(typeof(AWSCredentials)),
                    TypeFactory.GetTypeInfo(typeof(RegionEndpoint))
                });

            var newServiceClient = constructor.Invoke(new object[] { credentials, region }) as TClient;

            return newServiceClient;
        }

        public static TClient CreateServiceFromAssembly<TClient>(string assemblyName, string serviceClientClassName,
            AWSCredentials credentials, ClientConfig config)
            where TClient : class
        {
            var serviceClientTypeInfo = LoadServiceClientType(assemblyName, serviceClientClassName);
        
            var constructor = serviceClientTypeInfo.GetConstructor(new ITypeInfo[]
                {
                    TypeFactory.GetTypeInfo(typeof(AWSCredentials)),
                    TypeFactory.GetTypeInfo(config.GetType())
                });

            var newServiceClient = constructor.Invoke(new object[] { credentials, config }) as TClient;

            return newServiceClient;
        }

        public static ClientConfig CreateServiceConfig(string assemblyName, string serviceConfigClassName)
        {
            var typeInfo = LoadServiceConfigType(assemblyName, serviceConfigClassName);

            var ci = typeInfo.GetConstructor(new ITypeInfo[0]);
            var config = ci.Invoke(new object[0]);

            return config as ClientConfig;
        }

        private static ITypeInfo LoadServiceClientType(string assemblyName, string serviceClientClassName)
        {
            return LoadTypeFromAssembly(assemblyName, serviceClientClassName);
        }

        private static ITypeInfo LoadServiceConfigType(string assemblyName, string serviceConfigClassName)
        {
            return LoadTypeFromAssembly(assemblyName, serviceConfigClassName);
        }

        internal static ITypeInfo LoadTypeFromAssembly(string assemblyName, string className)
        {
            var assembly = GetSDKAssembly(assemblyName);
            var type = assembly.GetType(className);

            return TypeFactory.GetTypeInfo(type);
        }

        private static Assembly GetSDKAssembly(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => string.Equals(x.GetName().Name, assemblyName, StringComparison.Ordinal))
                ?? Assembly.Load(new AssemblyName(assemblyName))
                ?? throw new AmazonClientException($"Failed to load assembly. Be sure to include a reference to {assemblyName}.");
        }
    }
}
