namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// ILoggingBuilder extensions
    /// </summary>
    public static class ILoggerBuilderExtensions
    {
        /// <summary>
        /// Adds a Lambda logger provider with options loaded from the specified subsection of the
        /// configuration section.
        /// </summary>
        /// <param name="builder">ILoggingBuilder to add Lambda logger to.</param>
        /// <param name="configuration">IConfiguration to use when construction logging options.</param>
        /// <param name="loggingSectionName">Name of the logging section with required settings.</param>
        /// <returns>Updated ILoggingBuilder.</returns>
        public static ILoggingBuilder AddLambdaLogger(this ILoggingBuilder builder)
        {
            var provider = new LambdaILoggerProvider();
            builder.AddProvider(provider);
            return builder;
        }
    }
}
