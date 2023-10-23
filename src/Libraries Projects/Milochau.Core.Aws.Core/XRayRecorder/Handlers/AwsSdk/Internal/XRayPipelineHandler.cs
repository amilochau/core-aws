using System;
using System.Collections.Generic;
using Milochau.Core.Aws.Core.XRayRecorder.Core;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System.Linq;
using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.References;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// The handler to register <see cref="AmazonServiceClient"/> which can intercept downstream requests and responses.
    /// Note: This class should not be instantiated or used in anyway. It is used internally within SDK.
    /// </summary>
    public class XRayPipelineHandler
    {
        /// <summary>
        /// Removes amazon prefix from service name. There are two type of service name.
        ///     Amazon.DynamoDbV2
        ///     AmazonS3
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>String after removing Amazon prefix.</returns>
        private static string RemoveAmazonPrefixFromServiceName(string serviceName)
        {
            return RemovePrefix(RemovePrefix(serviceName, "Amazon"), ".");
        }

        private static string RemovePrefix(string originalString, string prefix)
        {
            if (originalString.StartsWith(prefix))
            {
                return originalString.Substring(prefix.Length);
            }

            return originalString;
        }

        private static string RemoveSuffix(string originalString, string suffix)
        {
            if (originalString.EndsWith(suffix))
            {
                return originalString.Substring(0, originalString.Length - suffix.Length);
            }

            return originalString;
        }

        /// <summary>
        /// Processes Begin request by starting subsegment.
        /// </summary>
        public static Subsegment ProcessBeginRequest(FacadeSegment facadeSegment, IRequestContext requestContext)
        {
            var serviceName = RemoveAmazonPrefixFromServiceName(requestContext.ClientConfig.MonitoringServiceName);
            var subsegment = AWSXRayRecorder.BeginSubsegment(facadeSegment, AWSXRaySDKUtils.FormatServiceName(serviceName));
            subsegment.Namespace = "aws";

            if (TraceHeader.TryParse(facadeSegment, out TraceHeader? traceHeader))
            {
                requestContext.HttpRequestMessage!.Headers.Add(TraceHeader.HeaderKey, traceHeader.ToString());
            }
            return subsegment;
        }

        /// <summary>
        /// Processes End request by ending subsegment.
        /// </summary>
        public static void ProcessEndRequest(Subsegment subsegment, IRequestContext requestContext, IResponseContext? responseContext)
        {
            var operation = RemoveSuffix(requestContext.OriginalRequest.GetType().Name, "Request");

            subsegment.AddToAws("region", EnvironmentVariables.RegionName);
            subsegment.AddToAws("operation", operation);
            if (responseContext.Response == null)
            {
                if (requestContext.HttpRequestMessage!.Headers.TryGetValues("x-amzn-RequestId", out var requestIds))
                {
                    subsegment.AddToAws("request_id", requestIds.First());
                }
                // s3 doesn't follow request header id convention
                else
                {
                    if (requestContext.HttpRequestMessage!.Headers.TryGetValues("x-amz-request-id", out requestIds))
                    {
                        subsegment.AddToAws("request_id", requestIds.First());
                    }

                    if (requestContext.HttpRequestMessage!.Headers.TryGetValues("x-amz-id-2", out requestIds))
                    {
                        subsegment.AddToAws("id_2", requestIds.First());
                    }
                }
            }
            else if (responseContext.Response.ResponseMetadata != null)
            {
                subsegment.AddToAws("request_id", responseContext.Response.ResponseMetadata.RequestId);

                // try getting x-amz-id-2 if dealing with s3 request
                if (responseContext.Response.ResponseMetadata.Metadata.TryGetValue("x-amz-id-2", out string? extendedRequestId))
                {
                    subsegment.AddToAws("id_2", extendedRequestId);
                }

                AddResponseSpecificInformation(responseContext.Response, subsegment);
            }

            AddHttpInformation(responseContext.HttpResponse, subsegment);
            AddRequestSpecificInformation(requestContext.OriginalRequest, subsegment);
            AWSXRayRecorder.EndSubsegment(subsegment);
        }

        private static void AddHttpInformation(HttpResponseMessage httpResponse, Subsegment subsegment)
        {
            var responseAttributes = new Dictionary<string, long>();
            int statusCode = (int)httpResponse.StatusCode;
            if (statusCode >= 400 && statusCode <= 499)
            {
                subsegment.MarkError();

                if (statusCode == 429)
                {
                    subsegment.MarkThrottle();
                }
            }
            else if (statusCode >= 500 && statusCode <= 599)
            {
                subsegment.MarkFault();
            }

            responseAttributes["status"] = statusCode;
            responseAttributes["content_length"] = httpResponse.Content.Headers.ContentLength ?? 0;
            subsegment.AddToHttp("response", responseAttributes);
        }

        private static void ProcessException(AmazonServiceException ex, Subsegment subsegment)
        {
            int statusCode = (int)ex.StatusCode;
            var responseAttributes = new Dictionary<string, long>();

            if (statusCode >= 400 && statusCode <= 499)
            {
                subsegment.MarkError();
                if (statusCode == 429)
                {
                    subsegment.MarkThrottle();
                }
            }
            else if (statusCode >= 500 && statusCode <= 599)
            {
                subsegment.MarkFault();
            }

            responseAttributes["status"] = statusCode;
            subsegment.AddToHttp("response", responseAttributes);

            subsegment.AddToAws("request_id", ex.RequestId);
        }

        private static void AddRequestSpecificInformation(AmazonWebServiceRequest request, Entity entity)
        {
            var xrayRequestParameters = request.GetXRayRequestParameters();
            foreach (var item in xrayRequestParameters.Where(x => x.Value != null))
            {
                entity.AddToAws(item.Key, item.Value);
            }
        }

        private static void AddResponseSpecificInformation(AmazonWebServiceResponse response, Entity entity)
        {
            var xrayResponseParameters = response.GetXRayResponseParameters();
            foreach (var item in xrayResponseParameters.Where(x => x.Value != null))
            {
                entity.AddToAws(item.Key, item.Value);
            }
        }

        public static void PopulateException(Subsegment subsegment, Exception e)
        {
            subsegment.AddException(e); // record exception 

            if (e is AmazonServiceException amazonServiceException)
            {
                ProcessException(amazonServiceException, subsegment);
            }
            return;
        }
    }
}
