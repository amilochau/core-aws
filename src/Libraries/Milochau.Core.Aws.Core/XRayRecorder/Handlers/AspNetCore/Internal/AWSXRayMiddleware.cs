using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Text;
using Milochau.Core.Aws.Core.XRayRecorder.Core;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.Core.References;

namespace Milochau.Core.Aws.Core.XRayRecorder.Handlers.AspNetCore.Internal
{
    /// <summary>
    /// The Middleware to intercept HTTP request for ASP.NET Core.
    /// For each request, <see cref="AWSXRayMiddleware"/> will try to parse trace header
    /// from HTTP request header, and determine if tracing is enabled. If enabled, it will
    /// start a new segment before invoking inner handler. And end the segment before it returns
    /// the response to outer handler.
    /// Note: This class should not be instantiated or used in anyway. It is used internally within SDK.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AWSXRayMiddleware"/> class.
    /// </remarks>
    /// <param name="next">Instance of <see cref="RequestDelegate"/></param>
    public class AWSXRayMiddleware(RequestDelegate next)
    {
        private readonly AWSXRayRecorder recorder = AWSXRayRecorder.Instance;
        private static readonly string X_FORWARDED_FOR = "X-Forwarded-For";
        private const string SchemeDelimiter = "://";

        /// <summary>
        /// Processes HTTP request and response. A segment is created at the beginning of the request and closed at the 
        /// end of the request. If the web app is running on AWS Lambda, a subsegment is started and ended for the respective 
        /// events.
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            ProcessHttpRequest(context);

            try
            {
                if (next != null)
                {
                    await next.Invoke(context); // call next handler
                }
            }
            catch (Exception exc)
            {
                recorder.AddException(exc);
                throw;
            }

            finally
            {
                ProcessHttpResponse(context);
            }
        }

        /// <summary>
        /// Processes HTTP response
        /// </summary>
        private void ProcessHttpResponse(HttpContext context)
        {
            HttpResponse response = context.Response;

            var responseAttributes = new Dictionary<string, object>();
            PopulateResponseAttributes(response, responseAttributes);
            recorder.AddHttpInformation("response", responseAttributes);

            recorder.EndSubsegment();
        }

        private void PopulateResponseAttributes(HttpResponse response, Dictionary<string, object> responseAttributes)
        {
            int statusCode = (int)response.StatusCode;

            if (statusCode >= 400 && statusCode <= 499)
            {
                recorder.MarkError();

                if (statusCode == 429)
                {
                    recorder.MarkThrottle();
                }
            }
            else if (statusCode >= 500 && statusCode <= 599)
            {
                recorder.MarkFault();
            }

            responseAttributes["status"] = statusCode;

            if (response.Headers.ContentLength != null)
            {
                responseAttributes["content_length"] = response.Headers.ContentLength;
            }
        }

        /// <summary>
        /// Processes HTTP request.
        /// </summary>
        private void ProcessHttpRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            string? headerString = null;

            if (request.Headers.TryGetValue(TraceHeader.HeaderKey, out StringValues headerValue))
            {
                if (headerValue.ToArray().Length >= 1)
                {
                    headerString = headerValue.ToArray()[0];
                }
            }

            // Trace header doesn't exist, which means this is the root node. Create a new traceId and inject the trace header.
            if (!TraceHeader.TryParse(headerString, out TraceHeader? traceHeader))
            {
                traceHeader = new TraceHeader
                {
                    RootTraceId = TraceId.NewId(),
                    ParentId = null,
                    Sampled = SampleDecision.Unknown
                };
            }

            var segmentName = EnvironmentVariables.FunctionName;
            bool isSampleDecisionRequested = traceHeader.Sampled == SampleDecision.Requested;

            // Make sample decision
            if (traceHeader.Sampled == SampleDecision.Unknown || traceHeader.Sampled == SampleDecision.Requested)
            {
                traceHeader.Sampled = recorder.SamplingStrategy.ShouldTrace();
            }

            recorder.BeginSubsegment(segmentName);

            var requestAttributes = new Dictionary<string, object?>();
            PopulateRequestAttributes(request, requestAttributes);
            recorder.AddHttpInformation("request", requestAttributes);

            if (isSampleDecisionRequested)
            {
                context.Response.Headers.Append(TraceHeader.HeaderKey, traceHeader.ToString()); // Its recommended not to modify response header after _next.Invoke() call
            }
        }

        private static void PopulateRequestAttributes(HttpRequest request, Dictionary<string, object?> requestAttributes)
        {
            requestAttributes["url"] = GetUrl(request);
            requestAttributes["method"] = request.Method;
            string? xForwardedFor = GetXForwardedFor(request);

            if (xForwardedFor == null)
            {
                requestAttributes["client_ip"] = GetClientIpAddress(request);
            }
            else
            {
                requestAttributes["client_ip"] = xForwardedFor;
                requestAttributes["x_forwarded_for"] = true;
            }

            if (request.Headers.TryGetValue(HeaderNames.UserAgent, out StringValues value))
            {
                requestAttributes["user_agent"] = value.ToString();
            }
        }

        // Implementing custom logic : https://github.com/aws/aws-xray-sdk-dotnet/issues/64
        private static string? GetUrl(HttpRequest? request)
        {
            if (request == null)
            {
                return null;
            }
            var scheme = request.Scheme ?? string.Empty;
            var host = request.Host.Value ?? string.Empty;
            var pathBase = request.PathBase.Value ?? string.Empty;
            var path = request.Path.Value ?? string.Empty;
            var queryString = request.QueryString.Value ?? string.Empty;

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length + path.Length + queryString.Length;

            return new StringBuilder(length)
                .Append(scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString();
        }

        private static string? GetXForwardedFor(HttpRequest request)
        {
            string? clientIp = null;

            if (request.HttpContext.Request.Headers.TryGetValue(X_FORWARDED_FOR, out StringValues headerValue))
            {
                if (headerValue.ToArray().Length >= 1)
                    clientIp = headerValue.ToArray()[0];
            }

            return string.IsNullOrEmpty(clientIp) ? null : clientIp.Split(',')[0].Trim();
        }

        private static string? GetClientIpAddress(HttpRequest request)
        {
            return request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
