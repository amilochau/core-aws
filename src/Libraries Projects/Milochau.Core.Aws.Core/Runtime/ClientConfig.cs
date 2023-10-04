using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// This class is the base class of all the configurations settings to connect
    /// to a service.
    /// </summary>
    public abstract partial class ClientConfig
    {
        /// <summary>
        /// Gets and sets the DisableLogging. If true logging for this client will be disabled.
        /// </summary>
        public bool DisableLogging { get; set; }
    }
}
