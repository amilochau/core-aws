using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using ExecutionContext = Amazon.Runtime.Internal.ExecutionContext;
using Amazon.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using Milochau.Core.Aws.Core.Runtime.Pipeline.RetryHandler;
using Milochau.Core.Aws.Core.Runtime.Pipeline.HttpHandler;
using Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers;
using Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.Core.Util;
using Milochau.Core.Aws.Core.Runtime.Internal.Auth;

namespace Milochau.Core.Aws.Core.Runtime
{
    public abstract class AmazonServiceClient : IDisposable
    {
        private bool _disposed;
        protected RuntimePipeline RuntimePipeline { get; set; }
        public IClientConfig Config { get; }

        #region Constructors

        protected AmazonServiceClient(ClientConfig config)
        {
            Config = config;
            Signer = new AWSSigner();
            BuildRuntimePipeline();
        }

        protected AWSSigner Signer { get; private set; }

        #endregion

        #region Invoke methods

        protected System.Threading.Tasks.Task<TResponse> InvokeAsync<TResponse>(
            AmazonWebServiceRequest request,
            InvokeOptionsBase options,
            System.Threading.CancellationToken cancellationToken)
            where TResponse : AmazonWebServiceResponse, new()
        {
            ThrowIfDisposed();

            var executionContext = new ExecutionContext(
                new RequestContext(Signer, Config, options.RequestMarshaller, options.ResponseUnmarshaller, request, cancellationToken),
                new ResponseContext()
            );
            return RuntimePipeline.InvokeAsync<TResponse>(executionContext);
        }

        #endregion

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                RuntimePipeline?.Dispose();

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

        private void BuildRuntimePipeline()
        {
            var httpHandler = new HttpHandler();

            //Determine which retry policy to use based on the retry mode
            RetryPolicy retryPolicy = new StandardRetryPolicy(Config);

            // Build default runtime pipeline.
            RuntimePipeline = new RuntimePipeline(new List<IPipelineHandler>
                {
                    httpHandler,
                    new Unmarshaller(),
                    new ErrorHandler(),
                    new Signer(),
                    // ChecksumHandler must come after CompressionHandler because we must calculate the checksum of a payload after compression.
                    // ChecksumHandler must come after EndpointsResolver because of an upcoming project.
                    new ChecksumHandler(),
                    new RetryHandler(retryPolicy),
                    new EndpointResolver(),
                    new Marshaller(),
                }
            );

            // Apply global pipeline customizations
            RuntimePipelineCustomizerRegistry.Instance.ApplyCustomizations(RuntimePipeline);
        }

        /// <summary>
        /// Assembles the Uri for a given SDK request
        /// </summary>
        /// <param name="iRequest">Request to compute Uri for</param>
        /// <returns>Uri for the given SDK request</returns>
        public static Uri ComposeUrl(IRequest iRequest)
        {
            return ComposeUrl(iRequest, true);
        }

        /// <summary>
        /// Assembles the Uri for a given SDK request
        /// </summary>
        /// <param name="internalRequest">Request to compute Uri for</param>
        /// <param name="skipEncodingValidPathChars">If true the accepted path characters {/+:} are not encoded.</param>
        /// <returns>Uri for the given SDK request</returns>
        public static Uri ComposeUrl(IRequest internalRequest, bool skipEncodingValidPathChars)
        {
            Uri url = internalRequest.Endpoint;
            var resourcePath = internalRequest.ResourcePath;
            if (resourcePath == null)
                resourcePath = string.Empty;
            else
            {
                if (resourcePath.StartsWith("/", StringComparison.Ordinal))
                    resourcePath = resourcePath.Substring(1);

                resourcePath = AWSSDKUtils.ResolveResourcePath(resourcePath, internalRequest.PathResources, skipEncodingValidPathChars);
            }

            // Construct any sub resource/query parameter additions to append to the
            // resource path. Services like S3 which allow '?' and/or '&' in resource paths 
            // should use SubResources instead of appending them to the resource path with 
            // query string delimiters during request marshalling.

            var sb = new StringBuilder();

            var parameterizedPath = string.Concat(resourcePath, sb);

            var hasSlash = url.AbsoluteUri.EndsWith("/", StringComparison.Ordinal) || parameterizedPath.StartsWith("/", StringComparison.Ordinal);
            var uri = hasSlash
                ? new Uri(url.AbsoluteUri + parameterizedPath)
                : new Uri(url.AbsoluteUri + "/" + parameterizedPath);
            return uri;
        }
    }
}
