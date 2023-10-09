using Milochau.Core.Aws.Core.Runtime.Internal.Util;
using Milochau.Core.Aws.Core.Util;
using System;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.RetryHandler
{
    /// <summary>
    /// The retry handler has the generic logic for retrying requests.
    /// It uses a retry policy which specifies when 
    /// a retry should be performed.
    /// </summary>
    public class RetryHandler : PipelineHandler
    {
        /// <summary>
        /// The retry policy which specifies when 
        /// a retry should be performed.
        /// </summary>
        public RetryPolicy RetryPolicy { get; private set; }
        
        /// <summary>
        /// Constructor which takes in a retry policy.
        /// </summary>
        /// <param name="retryPolicy">Retry Policy</param>
        public RetryHandler(RetryPolicy retryPolicy)
        {
            RetryPolicy = retryPolicy;
        }

        /// <summary>
        /// Invokes the inner handler and performs a retry, if required as per the
        /// retry policy.
        /// </summary>
        /// <typeparam name="T">The response type for the current request.</typeparam>
        /// <param name="executionContext">The execution context, it contains the
        /// request and response context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            bool shouldRetry = false;
            await RetryPolicy.ObtainSendTokenAsync(executionContext, null).ConfigureAwait(false);

            do
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo capturedException;

                try
                {        
                    SetRetryHeaders(requestContext);
                    T result = await base.InvokeAsync<T>(executionContext).ConfigureAwait(false);
                    RetryPolicy.NotifySuccess(executionContext);
                    return result;
                }
                catch (Exception e)
                {
                    capturedException = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e);
                }

                if (capturedException != null)
                {
                    shouldRetry = await RetryPolicy.RetryAsync(executionContext, capturedException.SourceException).ConfigureAwait(false);
                    if (!shouldRetry)
                    {
                        capturedException.Throw();
                    }
                    else
                    {
                        requestContext.Retries++;
                    }

                    await RetryPolicy.ObtainSendTokenAsync(executionContext, capturedException.SourceException).ConfigureAwait(false);
                }

                PrepareForRetry(requestContext);

                await RetryPolicy.WaitBeforeRetryAsync(executionContext).ConfigureAwait(false);

            } while (shouldRetry);
            throw new AmazonClientException("Neither a response was returned nor an exception was thrown in the Runtime RetryHandler.");
        }

        /// <summary>
        /// Prepares the request for retry.
        /// </summary>
        /// <param name="requestContext">Request context containing the state of the request.</param>
        internal static void PrepareForRetry(IRequestContext requestContext)
        {
            if (requestContext.Request.ContentStream != null &&
                requestContext.Request.OriginalStreamPosition >= 0)
            {
                var originalStream = requestContext.Request.ContentStream;
                var seekableStream = originalStream;

                // If the stream is wrapped in a HashStream, reset the HashStream
                if (originalStream is HashStream hashStream)
                {
                    hashStream.Reset();
                    seekableStream = hashStream.GetSeekableBaseStream();
                }
                seekableStream.Position = requestContext.Request.OriginalStreamPosition;
            }
        }
        
        private void SetRetryHeaders(IRequestContext requestContext)
        {    
            var request = requestContext.Request;

            //The invocation id will be the same for all retry requests for the initial operation invocation.
            if (!request.Headers.ContainsKey(HeaderKeys.AmzSdkInvocationId))
            {        
                request.Headers.Add(HeaderKeys.AmzSdkInvocationId, requestContext.InvocationId.ToString());
            }

            //Update the amz-sdk-request header with the current retry index.
            var requestPairs = $"attempt={requestContext.Retries + 1}; max={RetryPolicy.MaxRetries + 1}";

            if (request.Headers.ContainsKey(HeaderKeys.AmzSdkRequest))
            {
                request.Headers[HeaderKeys.AmzSdkRequest] = requestPairs;
            }
            else
            {
                request.Headers.Add(HeaderKeys.AmzSdkRequest, requestPairs);
            }
        }
    }
}
