using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.AspNetCoreServer
{
    /// <summary>
    /// The modes for when the ASP.NET Core framework will be initialized.
    /// </summary>
    public enum StartupMode
    {
        /// <summary>
        /// Initialize ASP.NET Core framework during the constructor
        /// </summary>
        Constructor,

        /// <summary>
        /// Initialize ASP.NET Core framework during the first incoming request
        /// </summary>
        FirstRequest
    }
}
