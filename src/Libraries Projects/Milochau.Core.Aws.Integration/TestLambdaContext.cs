using Milochau.Core.Aws.Core.Lambda.Core;

namespace Milochau.Core.Aws.Integration
{
    /// <summary>
    /// A test implementation of the ILambdaContext interface used for writing local tests of Lambda Functions.
    /// </summary>
    public class TestLambdaContext : ILambdaContext
    {
        /// <inheritdoc/>
        public string AwsRequestId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public ILambdaLogger Logger { get; set; } = new TestLambdaLogger();
    }
}
