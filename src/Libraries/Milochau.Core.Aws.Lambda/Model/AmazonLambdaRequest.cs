using Milochau.Core.Aws.Core.Runtime.Internal;
using System;

namespace Milochau.Core.Aws.Lambda.Model
{
    /// <summary>
    /// Base class for Lambda operation requests.
    /// </summary>
    public partial class AmazonLambdaRequest(Guid? userId) : AmazonWebServiceRequest(userId)
    {
    }
}
