using Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// This class is a container for extensions methods to the IWebHostBuilder
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Extension method for configuring Lambda as the server for an ASP.NET Core application.
        /// This is called instead of UseKestrel. If UseKestrel was called before this it will remove
        /// the service description that was added to the IServiceCollection.
        /// </summary>
        public static IWebHostBuilder UseLambdaServer(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(Utilities.EnsureLambdaServerRegistered);
        }
    }
}
