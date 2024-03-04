using Milochau.Core.Aws.Core.Runtime.Internal;
using System;

namespace Milochau.Core.Aws.SNS.Model
{
    /// <summary>
    /// Base class for SimpleNotificationService operation requests.
    /// </summary>
    public partial class AmazonSimpleNotificationServiceRequest(Guid? userId) : AmazonWebServiceRequest(userId)
    {
    }
}
