using System;

namespace Amazon.Runtime
{
    /// <summary>
    /// Represents how long a phase of an SDK request took.
    /// </summary>
    public interface IMetricsTiming
    {
        /// <summary>
        /// Whether the timing has been stopped
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Elapsed ticks from start to stop.
        /// If timing hasn't been stopped yet, returns 0.
        /// </summary>
        long ElapsedTicks { get; }

        /// <summary>
        /// Elapsed time from start to stop.
        /// If timing hasn't been stopped yet, returns TimeSpan.Zero
        /// </summary>
        TimeSpan ElapsedTime { get; }
    }

    /// <summary>
    /// Predefined request metrics that are collected by the SDK.
    /// </summary>
    public enum Metric
    {
        // response enums
        AWSErrorCode,
        AWSRequestID,
        AmzId2,
        BytesProcessed,
        Exception,
        RedirectLocation,
        ResponseProcessingTime,
        ResponseUnmarshallTime,
        ResponseReadTime,
        StatusCode,

        // request enums
        AttemptCount,
        CredentialsRequestTime,
        HttpRequestTime,
        ProxyHost,
        ProxyPort,
        RequestSigningTime,
        RetryPauseTime,
        StringToSign,
        CanonicalRequest,       
        // CSM metric added to measure the latency of each http 
        // request
        CSMAttemptLatency,

        // overall enums
        AsyncCall,
        ClientExecuteTime,
        MethodName,
        ServiceEndpoint,
        ServiceName,
        RequestSize,
        AmzCfId,

        RequestCompressionTime,
        UncompressedRequestSize,
    }
}
