using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Milochau.Core.Aws.XRayRecorder.Core;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.XRayRecorder.Core.Exceptions;
using System.Linq;
using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Pipeline.RetryHandler;
using Milochau.Core.Aws.Core.References;

namespace Milochau.Core.Aws.XRayRecorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// The handler to register <see cref="AmazonServiceClient"/> which can intercept downstream requests and responses.
    /// Note: This class should not be instantiated or used in anyway. It is used internally within SDK.
    /// </summary>
    public class XRayPipelineHandler : PipelineHandler
    {
        private readonly AWSXRayRecorder _recorder;

        /// <summary>
        /// Initializes a new instance of the <see cref="XRayPipelineHandler" /> class.
        /// </summary>
        public XRayPipelineHandler()
        {
            _recorder = AWSXRayRecorder.Instance;
        }

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
        private void ProcessBeginRequest(IExecutionContext executionContext)
        {
            var request = executionContext.RequestContext.Request!;
            Entity? entity = null;
            try
            {
                entity = _recorder.GetEntity();
            }
            catch (EntityNotAvailableException e)
            {
                _recorder.TraceContext.HandleEntityMissing(_recorder, e, "Cannot get entity while processing AWS SDK request");
            }

            var serviceName = RemoveAmazonPrefixFromServiceName(request.ServiceName);
            _recorder.BeginSubsegment(AWSXRaySDKUtils.FormatServiceName(serviceName));
            _recorder.SetNamespace("aws");

            entity = entity == null ? null : _recorder.GetEntity();

            if (TraceHeader.TryParse(entity!, out TraceHeader? traceHeader))
            {
                request.Headers[TraceHeader.HeaderKey] = traceHeader.ToString();
            }
        }

        /// <summary>
        /// Processes End request by ending subsegment.
        /// </summary>
        private void ProcessEndRequest(IExecutionContext executionContext)
        {
            Entity subsegment;
            try
            {
                subsegment = _recorder.GetEntity();
            }
            catch(EntityNotAvailableException e)
            {
                _recorder.TraceContext.HandleEntityMissing(_recorder,e,"Cannot get entity from the trace context while processing response of AWS SDK request.");
                return;
            }

            var responseContext = executionContext.ResponseContext;
            var requestContext = executionContext.RequestContext;

            if (responseContext == null)
            {
                return;
            }

            var client = executionContext.RequestContext.ClientConfig;
            if (client == null)
            {
                return;
            }

            var operation = RemoveSuffix(requestContext.OriginalRequest.GetType().Name, "Request");

            subsegment.AddToAws("region", EnvironmentVariables.RegionName);
            subsegment.AddToAws("operation", operation);
            if (responseContext.Response == null)
            {
                if (requestContext.Request!.Headers.TryGetValue("x-amzn-RequestId", out string? requestId))
                {
                    subsegment.AddToAws("request_id", requestId);
                }
                // s3 doesn't follow request header id convention
                else
                {
                    if (requestContext.Request.Headers.TryGetValue("x-amz-request-id", out requestId))
                    {
                        subsegment.AddToAws("request_id", requestId);
                    }

                    if (requestContext.Request.Headers.TryGetValue("x-amz-id-2", out requestId))
                    {
                        subsegment.AddToAws("id_2", requestId);
                    }
                }
            }
            else
            {
                subsegment.AddToAws("request_id", responseContext.Response.ResponseMetadata.RequestId);

                // try getting x-amz-id-2 if dealing with s3 request
                if (responseContext.Response.ResponseMetadata.Metadata.TryGetValue("x-amz-id-2", out string? extendedRequestId))
                {
                    subsegment.AddToAws("id_2", extendedRequestId);
                }

                AddResponseSpecificInformation(responseContext.Response, subsegment);
            }

            if (responseContext.HttpResponse != null)
            {
                AddHttpInformation(responseContext.HttpResponse);
            }

            AddRequestSpecificInformation(requestContext.OriginalRequest, subsegment);
            _recorder.EndSubsegment();
        }

        private void AddHttpInformation(IWebResponseData httpResponse)
        {
            var responseAttributes = new Dictionary<string, long>();
            int statusCode = (int)httpResponse.StatusCode;
            if (statusCode >= 400 && statusCode <= 499)
            {
                _recorder.MarkError();

                if (statusCode == 429)
                {
                    _recorder.MarkThrottle();
                }
            }
            else if (statusCode >= 500 && statusCode <= 599)
            {
                _recorder.MarkFault();
            }

            responseAttributes["status"] = statusCode;
            responseAttributes["content_length"] = httpResponse.ContentLength;
            _recorder.AddHttpInformation("response", responseAttributes);
        }

        private void ProcessException(AmazonServiceException ex, Entity subsegment)
        {
            int statusCode = (int)ex.StatusCode;
            var responseAttributes = new Dictionary<string, long>();

            if (statusCode >= 400 && statusCode <= 499)
            {
                _recorder.MarkError();
                if (statusCode == 429)
                {
                    _recorder.MarkThrottle();
                }
            }
            else if (statusCode >= 500 && statusCode <= 599)
            {
                _recorder.MarkFault();
            }

            responseAttributes["status"] = statusCode;
            _recorder.AddHttpInformation("response", responseAttributes);

            subsegment.AddToAws("request_id", ex.RequestId);
        }

        private static void AddRequestSpecificInformation(AmazonWebServiceRequest request, Entity entity)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var xrayRequestParameters = request.GetXRayRequestParameters();
            foreach (var item in xrayRequestParameters.Where(x => x.Value != null))
            {
                entity.AddToAws(item.Key, item.Value);
            }

            var xrayRequestDescriptors = request.GetXRayRequestDescriptors();
            foreach (var item in xrayRequestDescriptors.Where(x => x.Value != null))
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

        private void PopulateException(Exception e)
        {
            Entity subsegment;
            try
            {
                subsegment = _recorder.GetEntity();
            }
            catch (EntityNotAvailableException ex)
            {
                _recorder.TraceContext.HandleEntityMissing(_recorder, ex, "Cannot get entity from trace context while processing exception for AWS SDK request.");
                return;
            }

            subsegment.AddException(e); // record exception 

            if (e is AmazonServiceException amazonServiceException)
            {
                ProcessException(amazonServiceException, subsegment);
            }
            return;
        }

        private static bool ExcludeServiceOperation(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            var serviceName = RemoveAmazonPrefixFromServiceName(requestContext.Request!.ServiceName);
            var operation = RemoveSuffix(requestContext.OriginalRequest.GetType().Name, "Request");

            return AWSXRaySDKUtils.IsBlacklistedOperation(serviceName,operation);
        }

        /// <summary>
        /// Process Asynchronous <see cref="AmazonServiceClient"/> operations. A subsegment is started at the beginning of 
        /// the request and ended at the end of the request.
        /// </summary>
        public override async Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            T? ret = null;

            if (ExcludeServiceOperation(executionContext))
            {
                ret = await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
            }
            else
            {
                ProcessBeginRequest(executionContext);

                try
                {
                    ret = await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    PopulateException(e);
                    throw;
                }
                finally
                {
                    ProcessEndRequest(executionContext);
                }
            }

            return ret;
        }
    }

    /// <summary>
    /// Pipeline Customizer for registering <see cref="AmazonServiceClient"/> instances with AWS X-Ray.
    /// Note: This class should not be instantiated or used in anyway. It is used internally within SDK.
    /// </summary>
    public class XRayPipelineCustomizer : IRuntimePipelineCustomizer
    {
        /// <summary>Unique name</summary>
        public string UniqueName { get { return "X-Ray Registration Customization"; } }

        /// <summary>Customize</summary>
        public void Customize(RuntimePipeline pipeline)
        {
            pipeline.AddHandlerBefore<RetryHandler>(new XRayPipelineHandler());
        }
    }
}
