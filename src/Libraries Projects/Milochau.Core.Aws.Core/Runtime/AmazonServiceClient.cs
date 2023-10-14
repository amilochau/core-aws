using System;
using System.Collections.Generic;
using ExecutionContext = Amazon.Runtime.Internal.ExecutionContext;
using Amazon.Runtime.Internal;
using Milochau.Core.Aws.Core.Runtime.Pipeline;
using Milochau.Core.Aws.Core.Runtime.Pipeline.HttpHandler;
using Milochau.Core.Aws.Core.Runtime.Pipeline.Handlers;
using Milochau.Core.Aws.Core.Runtime.Pipeline.ErrorHandler;
using Milochau.Core.Aws.Core.Runtime.Internal;
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
            InvokeOptions options,
            System.Threading.CancellationToken cancellationToken)
            where TResponse : AmazonWebServiceResponse, new()
        {
            ThrowIfDisposed();

            var executionContext = new ExecutionContext(
                new RequestContext(Signer, Config, options.HttpRequestMessageMarshaller, options.ResponseUnmarshaller, request, cancellationToken),
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
            // Build default runtime pipeline.
            RuntimePipeline = new RuntimePipeline(new List<IPipelineHandler>
                {
                    new HttpHandler(),
                    new Unmarshaller(),
                    new ErrorHandler(),
                    new Signer(),
                    new Marshaller(),
                }
            );

            // Apply global pipeline customizations
            RuntimePipelineCustomizerRegistry.Instance.ApplyCustomizations(RuntimePipeline);
        }
    }
}
