namespace Amazon.Lambda.Core
{
    /// <summary>
    /// Object that allows you to access useful information available within
    /// the Lambda execution environment.
    /// </summary>
    public interface ILambdaContext
    {
        /// <summary>
        /// The AWS request ID associated with the request.
        /// This is the same ID returned to the client that called invoke().
        /// This ID is reused for retries on the same request.
        /// </summary>
        string AwsRequestId { get; }

        /// <summary>
        /// Lambda logger associated with the Context object.
        /// </summary>
        ILambdaLogger Logger { get; }
    }
}