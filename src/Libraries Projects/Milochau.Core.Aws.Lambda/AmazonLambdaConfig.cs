using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Lambda.Internal;

namespace Milochau.Core.Aws.Lambda
{
    /// <summary>
    /// Configuration for accessing Amazon Lambda service
    /// </summary>
    public partial class AmazonLambdaConfig : ClientConfig
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AmazonLambdaConfig()
            : base()
        {
            AuthenticationServiceName = "lambda";
            EndpointProvider = new AmazonLambdaEndpointProvider();
        }
    }
}