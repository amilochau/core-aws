using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Core/Amazon.Runtime/Internal/InvokeOptions.cs
namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    public abstract class InvokeOptionsBase
    {
        protected InvokeOptionsBase()
        {
        }
    }

    /// <summary>
    /// Class containing the members used to invoke service calls
    /// <para>
    /// This class is only intended for internal use inside the AWS client libraries.
    /// Callers shouldn't ever interact directly with objects of this class.
    /// </para>
    /// </summary>
    public class InvokeOptions : InvokeOptionsBase
    {
        public InvokeOptions() : base()
        {
        }
    }
}
