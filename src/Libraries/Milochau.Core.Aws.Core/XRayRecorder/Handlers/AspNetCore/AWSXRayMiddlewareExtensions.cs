using Milochau.Core.Aws.Core.XRayRecorder.Handlers.AspNetCore.Internal;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// The Middleware Extension to intercept HTTP request for ASP.NET Core.
    /// For each request, <see cref="AWSXRayMiddleware"/> will try to parse trace header
    /// from HTTP request header, and determine if tracing is enabled. If enabled, it will
    /// start a new segment before invoking inner handler. And end the segment before it returns
    /// the response to outer handler.
    /// </summary>
    public static class AWSXRayMiddlewareExtensions
    {
        /// <summary>
        /// Adds <see cref="AWSXRayMiddleware"/> to the applicaion's request pipeline.
        /// </summary>
        /// <param name="builder">Instance of <see cref="IApplicationBuilder"/>.</param>
        /// <returns>Instance of <see cref="IApplicationBuilder"/> instrumented with X-Ray middleware.</returns>
        public static IApplicationBuilder UseXRay(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AWSXRayMiddleware>();
        }
    }
}
