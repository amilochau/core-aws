using Amazon.Lambda.Core;

namespace Amazon.Lambda.TestUtilities
{
    /// <summary>
    /// A test implementation of the ILambdaContext interface used for writing local tests of Lambda Functions.
    /// </summary>
    public class TestLambdaContext : ILambdaContext
    {
        /// <summary>
        /// The AWS request ID associated with the request.
        /// </summary>
        public string? AwsRequestId { get; set; }

        /// <summary>
        /// Lambda logger associated with the Context object. For the TestLambdaContext this is default to the TestLambdaLogger.
        /// </summary>
        public ILambdaLogger Logger { get; set; } = new TestLambdaLogger();
    }
}
