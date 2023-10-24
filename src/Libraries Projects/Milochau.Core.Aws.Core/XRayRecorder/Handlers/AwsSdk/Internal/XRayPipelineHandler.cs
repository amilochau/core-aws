using System;
using System.Collections.Generic;
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
        /// <summary>Processes Begin request by starting subsegment.</summary>
        public static Subsegment ProcessBeginRequest(FacadeSegment facadeSegment, IRequestContext requestContext)
        {
            // Create subsegment
            var subsegment = new Subsegment(requestContext.ClientConfig.MonitoringServiceName, facadeSegment);
            facadeSegment.Subsegments.Add(subsegment);

            // Add trace headers to request headers
            if (TraceHeader.TryParse(facadeSegment, out TraceHeader? traceHeader))
            {
                requestContext.HttpRequestMessage!.Headers.Add(TraceHeader.HeaderKey, traceHeader.ToString());
            }
            return subsegment;
        }

        /// <summary>Populate exception.</summary>
        public static void PopulateException(Subsegment subsegment, Exception e)
        {
            subsegment.AddException(e);

            if (e is AmazonServiceException amazonServiceException)
            {
                ProcessException(amazonServiceException, subsegment);
            }
        }

        /// <summary>Processes End request by ending subsegment.</summary>
        public static void ProcessEndRequest(Subsegment subsegment, IRequestContext requestContext, IResponseContext? responseContext)
        {
            subsegment.AddToAws("region", EnvironmentVariables.RegionName);
            subsegment.AddToAws("operation", requestContext.MonitoringOriginalRequestName);
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
            subsegment.End();
        }

        private static void AddHttpInformation(HttpResponseMessage httpResponse, Subsegment subsegment)
        {
            int statusCode = (int)httpResponse.StatusCode;
            if (statusCode == 429)
            {
                subsegment.HasError = false;
                subsegment.HasFault = true;
                subsegment.IsThrottled = true;
            }
            else if (statusCode >= 400 && statusCode <= 499)
            {
                subsegment.HasError = true;
                subsegment.HasFault = false;
            }
            else if (statusCode >= 500 && statusCode <= 599)
            {
                subsegment.HasError = false;
                subsegment.HasFault = true;
            }

            var responseAttributes = new Dictionary<string, long>
            {
                ["status"] = statusCode,
                ["content_length"] = httpResponse.Content.Headers.ContentLength ?? 0,
            };
            subsegment.AddToHttp("response", responseAttributes);
        }

        private static void ProcessException(AmazonServiceException ex, Subsegment subsegment)
        {
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
    }
}
