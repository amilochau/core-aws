using System;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System.Linq;
using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.References;
using Amazon.Runtime.Internal;

namespace Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// The handler to register <see cref="AmazonServiceClient"/> which can intercept downstream requests and responses.
    /// Note: This class should not be instantiated or used in anyway. It is used internally within SDK.
    /// </summary>
    public class XRayPipelineHandler
    {
        /// <summary>Processes Begin request by starting subsegment.</summary>
        public static Subsegment ProcessBeginRequest(FacadeSegment facadeSegment, RequestContext requestContext)
        {
            // Create subsegment
            var subsegment = new Subsegment(requestContext.ClientConfig.MonitoringServiceName, facadeSegment)
            {
                Namespace = "aws",
            };
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
        public static void ProcessEndRequest(Subsegment subsegment, RequestContext requestContext, ResponseContext responseContext)
        {
            subsegment.Aws["region"] = EnvironmentVariables.RegionName;
            subsegment.Aws["operation"] = requestContext.MonitoringOriginalRequestName;
            if (responseContext.Response == null)
            {
                if (requestContext.HttpRequestMessage.Headers.TryGetValues("x-amzn-RequestId", out var requestIds))
                {
                    subsegment.Aws["request_id"] = requestIds.First();
                }
                // s3 doesn't follow request header id convention
                else
                {
                    if (requestContext.HttpRequestMessage.Headers.TryGetValues("x-amz-request-id", out requestIds))
                    {
                        subsegment.Aws["request_id"] = requestIds.First();
                    }

                    if (requestContext.HttpRequestMessage.Headers.TryGetValues("x-amz-id-2", out requestIds))
                    {
                        subsegment.Aws["id_2"] = requestIds.First();
                    }
                }
            }
            else if (responseContext.Response.ResponseMetadata != null)
            {
                subsegment.Aws["request_id"] = responseContext.Response.ResponseMetadata.RequestId;

                // try getting x-amz-id-2 if dealing with s3 request
                if (responseContext.Response.ResponseMetadata.Metadata.TryGetValue("x-amz-id-2", out string? extendedRequestId))
                {
                    subsegment.Aws["id_2"] = extendedRequestId;
                }

                AddResponseSpecificInformation(responseContext.Response, subsegment);
            }

            subsegment.AddHttpInformation(requestContext.HttpRequestMessage);
            if (responseContext.HttpResponse != null)
            {
                subsegment.AddHttpInformation(responseContext.HttpResponse);
            }
            AddRequestSpecificInformation(requestContext.OriginalRequest, subsegment);

            if (requestContext.OriginalRequest.UserId != null && requestContext.OriginalRequest.UserId.Value != default)
            {
                subsegment.Annotations["user_id"] = requestContext.OriginalRequest.UserId.Value.ToString("N");
            }

            subsegment.End();
        }

        private static void ProcessException(AmazonServiceException ex, Subsegment subsegment)
        {
            subsegment.Aws["request_id"] = ex.RequestId;
        }

        private static void AddRequestSpecificInformation(AmazonWebServiceRequest request, Subsegment subsegment)
        {
            var xrayRequestParameters = request.GetXRayRequestParameters();
            foreach (var item in xrayRequestParameters.Where(x => x.Value != null))
            {
                subsegment.Aws[item.Key] = item.Value;
            }
        }

        private static void AddResponseSpecificInformation(AmazonWebServiceResponse response, Subsegment subsegment)
        {
            var xrayResponseParameters = response.GetXRayResponseParameters();
            foreach (var item in xrayResponseParameters.Where(x => x.Value != null))
            {
                subsegment.Aws[item.Key] = item.Value;
            }
        }
    }
}
