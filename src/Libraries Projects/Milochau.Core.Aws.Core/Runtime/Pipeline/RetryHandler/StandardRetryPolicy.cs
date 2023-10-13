using System;
using System.Net;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime.Pipeline.RetryHandler
{
    /// <summary>
    /// The default implementation of the standard policy.
    /// </summary>
    public partial class StandardRetryPolicy : RetryPolicy
    {
        private static readonly Random _randomJitter = new Random();

        //The status code returned from a service request when an invalid endpoint is used.
        private const int INVALID_ENDPOINT_EXCEPTION_STATUSCODE = 421;

        protected static CapacityManager CapacityManagerInstance { get; set; } = new CapacityManager(throttleRetryCount: 100, throttleRetryCost: 5, throttleCost: 1, timeoutRetryCost: 10);

        /// <summary>
        /// The maximum value of exponential backoff in milliseconds, which will be used to wait
        /// before retrying a request. The default is 20000 milliseconds.
        /// </summary>
        public const int MaxBackoffInMilliseconds = 20000;

        /// <summary>
        /// Constructor for StandardRetryPolicy.
        /// </summary>
        /// <param name="config">The Client config object. This is used to 
        /// retrieve the maximum number of retries  before throwing
        /// back a exception(This does not count the initial request) and
        /// the service URL for the request.</param>
        public StandardRetryPolicy(IClientConfig config)
        {
            MaxRetries = config.MaxErrorRetry;
            RetryCapacity = CapacityManagerInstance.GetRetryCapacity(GetRetryCapacityKey(config));
        }

        /// <summary>
        /// Return true if the request should be retried.
        /// </summary>
        /// <param name="executionContext">Request context containing the state of the request.</param>
        /// <param name="exception">The exception thrown by the previous request.</param>
        /// <returns>Return true if the request should be retried.</returns>
        public override bool RetryForException(IExecutionContext executionContext, Exception exception)
        {
            return RetryForExceptionSync(exception, executionContext);
        }

        /// <summary>
        /// Waits before retrying a request.
        /// </summary>
        /// <param name="executionContext">The execution context which contains both the
        /// requests and response context.</param>
        public override Task WaitBeforeRetryAsync(IExecutionContext executionContext)
        {
            var delay = CalculateRetryDelay(executionContext.RequestContext.Retries);
            return Task.Delay(delay, executionContext.RequestContext.CancellationToken);
        }

        /// <summary>
        /// Returns true if the request is in a state where it can be retried, else false.
        /// </summary>
        /// <param name="executionContext">Request context containing the state of the request.</param>
        /// <returns>Returns true if the request is in a state where it can be retried, else false.</returns>
        public override bool CanRetry(IExecutionContext executionContext)
        {
            return executionContext.RequestContext.Request.IsRequestStreamRewindable();
        }

        /// <summary>
        /// Virtual method that gets called when a retry request is initiated. If retry throttling is
        /// enabled, the value returned is true if the required capacity is retured, false otherwise. 
        /// If retry throttling is disabled, true is returned.
        /// </summary>
        /// <param name="executionContext">The execution context which contains both the
        /// requests and response context.</param>
        /// <param name="bypassAcquireCapacity">true to bypass any attempt to acquire capacity on a retry</param>
        public override bool OnRetry(IExecutionContext executionContext, bool bypassAcquireCapacity)
        {
            return OnRetry(executionContext, bypassAcquireCapacity, false);
        }

        /// <summary>
        /// Virtual method that gets called when a retry request is initiated. If retry throttling is
        /// enabled, the value returned is true if the required capacity is retured, false otherwise. 
        /// If retry throttling is disabled, true is returned.
        /// </summary>
        /// <param name="executionContext">The execution context which contains both the
        /// requests and response context.</param>
        /// <param name="bypassAcquireCapacity">true to bypass any attempt to acquire capacity on a retry</param>
        /// <param name="isThrottlingError">true if the error that will be retried is a throtting error</param>        
        public override bool OnRetry(IExecutionContext executionContext, bool bypassAcquireCapacity, bool isThrottlingError)
        {
            if (!bypassAcquireCapacity && RetryCapacity != null)
            {
                return CapacityManagerInstance.TryAcquireCapacity(RetryCapacity, executionContext.RequestContext.LastCapacityType);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Virtual method that gets called on a success Response. If its a retry success response, the entire 
        /// retry acquired capacity is released(default is 5). If its just a success response a lesser value capacity 
        /// is released(default is 1).
        /// </summary>
        /// <param name="executionContext">Request context containing the state of the request.</param>
        public override void NotifySuccess(IExecutionContext executionContext)
        {
            if (RetryCapacity != null)
            {
                var requestContext = executionContext.RequestContext;
                CapacityManagerInstance.ReleaseCapacity(requestContext.LastCapacityType, RetryCapacity);
            }
        }

        /// <summary>
        /// Perform the processor-bound portion of the RetryForException logic.
        /// This is shared by the sync, async, and APM versions of the RetryForException method.
        /// </summary>
        /// <param name="exception">The exception thrown by the previous request.</param>
        /// <param name="executionContext">Request context containing the state of the request.</param>
        /// <returns>Return true if the request should be retried.</returns>
        protected bool RetryForExceptionSync(Exception exception, IExecutionContext executionContext)
        {
            // AmazonServiceException is thrown by ErrorHandler if it is this type of exception.
            var serviceException = exception as AmazonServiceException;

            // To try and smooth out an occasional throttling error, we'll pause and 
            // retry, hoping that the pause is long enough for the request to get through
            // the next time. Only the error code should be used to determine if an 
            // error is a throttling error.
            if (IsThrottlingError(exception))
            {
                return true;
            }

            // Check for transient errors, but we need to use
            // an exponential back-off strategy so that we don't overload
            // a server with a flood of retries. If we've surpassed our
            // retry limit we handle the error response as a non-retryable
            // error and go ahead and throw it back to the user as an exception.
            if (IsTransientError(executionContext, exception) || IsServiceTimeoutError(exception))
            {
                return true;
            }
            
            //Check for Invalid Endpoint Exception indicating that the Endpoint Discovery
            //endpoint used was invalid for the request. One retry attempt is allowed for this
            //type of exception.
            if (serviceException?.StatusCode == (HttpStatusCode)INVALID_ENDPOINT_EXCEPTION_STATUSCODE)
            {
                if (executionContext.RequestContext.EndpointDiscoveryRetries < 1)
                {
                    executionContext.RequestContext.EndpointDiscoveryRetries++;
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Checks if the retry limit is reached.
        /// </summary>
        /// <param name="executionContext">Request context containing the state of the request.</param>
        /// <returns>Return false if the request can be retried, based on number of retries.</returns>
        public override bool RetryLimitReached(IExecutionContext executionContext)
        {
            return executionContext.RequestContext.Retries >= MaxRetries;
        }

        protected static int CalculateRetryDelay(int retries)
        {
            double jitter;
            lock (_randomJitter) {        
                jitter = _randomJitter.NextDouble();
            }
            return Convert.ToInt32(Math.Min(jitter * Math.Pow(2, retries - 1) * 1000.0, MaxBackoffInMilliseconds));
        }
    }
}
