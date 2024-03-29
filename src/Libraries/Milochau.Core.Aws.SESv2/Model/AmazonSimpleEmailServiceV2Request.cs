using Milochau.Core.Aws.Core.Runtime.Internal;
using System;

namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// Base class for SimpleEmailServiceV2 operation requests.
    /// </summary>
    public partial class AmazonSimpleEmailServiceV2Request(Guid? userId) : AmazonWebServiceRequest(userId)
    {
    }
}