using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>Extension methods for IHostBuilder</summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures the default settings for IWebHostBuilder when running in Lambda. The major difference between ConfigureWebHostDefaults and ConfigureWebHostLambdaDefaults
        /// is that it calls "webBuilder.UseLambdaServer()" to swap out Kestrel for Lambda as the IServer.
        /// </summary>
        public static IHostBuilder ConfigureWebHostLambdaDefaults(this IHostBuilder builder)
        {
            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LAMBDA_TASK_ROOT")))
                        {
                            logging.AddConsole();
                            logging.AddDebug();
                        }
                        else
                        {
                            logging.ClearProviders();
                            logging.AddLambdaLogger(hostingContext.Configuration, "Logging");
                        }
                    })
                    .UseDefaultServiceProvider((hostingContext, options) =>
                    {
                        options.ValidateScopes = hostingContext.HostingEnvironment.IsDevelopment();
                    });

                // Swap out Kestrel as the webserver and use our implementation of IServer
                webBuilder.UseLambdaServer();
            });

            return builder;
        }
    }
}
