using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;
using Amazon.Runtime;
using Milochau.Core.Aws.XRayRecorder.Core;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.XRayRecorder.Core.Internal.Utils;
using Milochau.Core.Aws.XRayRecorder.Handlers.AwsSdk.Entities;
using Amazon.Runtime.Internal;
using System.Threading;
using Amazon.Runtime.Internal.Transform;
using Milochau.Core.Aws.XRayRecorder.Core.Exceptions;
using Milochau.Core.Aws.XRayRecorder.References;
using System.Reflection.Metadata;
using System.Linq;

namespace Milochau.Core.Aws.XRayRecorder.Handlers.AwsSdk.Internal
{
    /// <summary>
    /// The handler to register <see cref="Amazon.Runtime.AmazonServiceClient"/> which can intercept downstream requests and responses.
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

        private static bool TryReadPropertyValue(object obj, string propertyName, out object value)
        {
            value = 0;
            
            return false;

            /* @todo This code does not work as reflection is disabled, but we could find alternatives to trace:
             * - RequestDescriptors
             * - RequestParameters
             * - ResponseParameters

            try
            {
                if (obj == null || propertyName == null)
                {
                    return false;
                }

                var property = obj.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    return false;
                }

                var propertyValue = property.GetValue(obj);
                if (propertyValue == null)
                {
                    return false;
                }

                value = propertyValue;
                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            catch (AmbiguousMatchException)
            {
                return false;
            }
            */
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

        private static void AddMapKeyProperty(Entity entity, object obj, string propertyName, string? renameTo = null)
        {
            if (!TryReadPropertyValue(obj, propertyName, out object propertyValue))
            {
                return;
            }

            var dictionaryValue = propertyValue as IDictionary;

            if (dictionaryValue == null)
            {
                return;
            }

            var newPropertyName = string.IsNullOrEmpty(renameTo) ? propertyName : renameTo;
            entity.AddToAws(newPropertyName.FromCamelCaseToSnakeCase(), dictionaryValue.Keys);
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

            var serviceName = RemoveAmazonPrefixFromServiceName(requestContext.Request!.ServiceName);
            var operation = RemoveSuffix(requestContext.OriginalRequest.GetType().Name, "Request");

            subsegment.AddToAws("region", client.RegionEndpoint?.SystemName);
            subsegment.AddToAws("operation", operation);
            if (responseContext.Response == null)
            {
                if (requestContext.Request.Headers.TryGetValue("x-amzn-RequestId", out string? requestId))
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

                AddResponseSpecificInformation(serviceName, operation, responseContext.Response, subsegment);
            }

            if (responseContext.HttpResponse != null)
            {
                AddHttpInformation(responseContext.HttpResponse);
            }

            AddRequestSpecificInformation(serviceName, operation, requestContext.OriginalRequest, subsegment);
            _recorder.EndSubsegment();
        }

        private void AddHttpInformation(IWebResponseData httpResponse)
        {
            var responseAttributes = new Dictionary<string, object>();
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
            var responseAttributes = new Dictionary<string, object>();

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

        private void AddRequestSpecificInformation(string serviceName, string operation, AmazonWebServiceRequest request, Entity entity)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (!XRayServices.Instance.Services.TryGetValue(serviceName, out AWSServiceHandler? serviceHandler))
            {
                return;
            }

            if (!serviceHandler.Operations.TryGetValue(operation, out AWSOperationHandler? operationHandler))
            {
                return;
            }

            // @todo The following code is the new code, to improve traces with XRay from requests
            var xrayRequestParameters = request.GetXRayRequestParameters();
            foreach (var item in xrayRequestParameters.Where(x => x.Value != null))
            {
                entity.AddToAws(item.Key, item.Value);
            }

            /*
             * @todo The following code is the original code.
            if (operationHandler.RequestParameters != null)
            {
                foreach (string parameter in operationHandler.RequestParameters)
                {
                    if (TryReadPropertyValue(request, parameter, out object propertyValue))
                    {
                        entity.AddToAws(parameter.FromCamelCaseToSnakeCase(), propertyValue);
                    }
                }
            }
            */

            if (operationHandler.RequestDescriptors != null)
            {
                foreach (KeyValuePair<string, AWSOperationRequestDescriptor> kv in operationHandler.RequestDescriptors)
                {
                    var propertyName = kv.Key;
                    var descriptor = kv.Value;

                    if (descriptor.Map && descriptor.GetKeys)
                    {
                        AddMapKeyProperty(entity, request, propertyName, descriptor.RenameTo);
                    }
                }
            }
        }

        private void AddResponseSpecificInformation(string serviceName, string operation, AmazonWebServiceResponse response, Entity entity)
        {
            if (!XRayServices.Instance.Services.TryGetValue(serviceName, out AWSServiceHandler? serviceHandler))
            {
                return;
            }

            if (!serviceHandler.Operations.TryGetValue(operation, out AWSOperationHandler? operationHandler))
            {
                return;
            }

            if (operationHandler.ResponseParameters != null)
            {
                foreach (string parameter in operationHandler.ResponseParameters)
                {
                    if (TryReadPropertyValue(response, parameter, out object propertyValue))
                    {
                        entity.AddToAws(parameter.FromCamelCaseToSnakeCase(), propertyValue);
                    }
                }
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
