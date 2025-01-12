using System;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System.Linq;
using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.References;
using Amazon.Runtime.Internal;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Exceptions;
using Milochau.Core.Aws.Core.XRayRecorder.Core;
using System.Collections.Generic;
using System.Net.Http;

namespace Milochau.Core.Aws.Core.XRayRecorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// The handler to register <see cref="AmazonServiceClient"/> which can intercept downstream requests and responses.
    /// Note: This class should not be instantiated or used in anyway. It is used internally within SDK.
    /// </summary>
    public class XRayPipelineHandler
    {
        private readonly AWSXRayRecorder recorder = AWSXRayRecorder.Instance;

        /// <summary>
        /// Processes Begin request by starting subsegment.
        /// </summary>
        public void ProcessBeginRequest(string serviceName, HttpRequestMessage request)
        {
            Entity? entity = null;
            try
            {
                entity = recorder.GetEntity();
            }
            catch (EntityNotAvailableException)
            {
            }

            recorder.BeginSubsegment(AWSXRaySDKUtils.FormatServiceName(serviceName));
            recorder.SetNamespace("aws");

            entity = entity == null ? null : recorder.GetEntity();

            if (TraceHeader.TryParse(entity, out TraceHeader? traceHeader))
            {
                request.Headers.Add(TraceHeader.HeaderKey, traceHeader.ToString());
            }
        }

        /// <summary>
        /// Processes End request by ending subsegment.
        /// </summary>
        public void ProcessEndRequest(RequestContext requestContext, ResponseContext responseContext)
        {
            Entity subsegment;
            try
            {
                subsegment = recorder.GetEntity();
            }
            catch (EntityNotAvailableException)
            {
                return;
            }

            if (responseContext == null)
            {
                return;
            }

            if (requestContext.ClientConfig == null)
            {
                return;
            }
            
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
            else if(responseContext.Response.ResponseMetadata != null)
            {
                subsegment.Aws["request_id"] = responseContext.Response.ResponseMetadata.RequestId;

                // try getting x-amz-id-2 if dealing with s3 request
                if (responseContext.Response.ResponseMetadata.Metadata.TryGetValue("x-amz-id-2", out string? extendedRequestId))
                {
                    subsegment.Aws["id_2"] = extendedRequestId;
                }

                AddResponseSpecificInformation(responseContext.Response, subsegment.Aws);
            }

            if (responseContext.HttpResponse != null)
            {
                AddHttpInformation(responseContext.HttpResponse);
            }

            AddRequestSpecificInformation(requestContext.OriginalRequest, subsegment.Aws);
            recorder.EndSubsegment();
        }

        private void AddHttpInformation(HttpResponseMessage httpResponse)
        {
            var responseAttributes = new Dictionary<string, object>();
            int statusCode = (int)httpResponse.StatusCode;
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
            responseAttributes["content_length"] = httpResponse.Content.Headers.ContentLength ?? 0;
            recorder.AddHttpInformation("response", responseAttributes);
        }

        private void ProcessException(AmazonServiceException ex, Entity subsegment)
        {
            int statusCode = (int)ex.StatusCode;
            var responseAttributes = new Dictionary<string, object>();

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
            recorder.AddHttpInformation("response", responseAttributes);

            subsegment.Aws["request_id"] = ex.RequestId ?? string.Empty; // @todo should not be null
        }

        private void AddRequestSpecificInformation(AmazonWebServiceRequest request, IDictionary<string, object?> aws)
        {
            var xrayRequestParameters = request.GetXRayRequestParameters();
            foreach (var item in xrayRequestParameters.Where(x => x.Value != null))
            {
                aws[item.Key] = item.Value;
            }

            var xrayRequestDescriptors = request.GetXRayRequestDescriptors();
            foreach (var item in xrayRequestDescriptors.Where(x => x.Value != null))
            {
                aws[item.Key] = item.Value;
            }
        }

        private void AddResponseSpecificInformation(AmazonWebServiceResponse response, IDictionary<string, object?> aws)
        {
            var xrayResponseParameters = response.GetXRayResponseParameters();
            foreach (var item in xrayResponseParameters.Where(x => x.Value != null))
            {
                aws[item.Key] = item.Value;
            }

            var xrayResponseDescriptors = response.GetXRayResponseDescriptors();
            foreach (var item in xrayResponseDescriptors.Where(x => x.Value != null))
            {
                aws[item.Key] = item.Value;
            }
        }

        public void PopulateException(Exception e)
        {
            Entity subsegment;
            try
            {
                subsegment = recorder.GetEntity();
            }
            catch (EntityNotAvailableException)
            {
                return;
            }

            subsegment.AddException(e); // record exception 

            if (e is AmazonServiceException amazonServiceException)
            {
                ProcessException(amazonServiceException, subsegment);
            }
            return;
        }
    }
}
