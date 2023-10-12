using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.References
{
    /// <summary>
    /// Log Level for logging messages
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Trace level logging
        /// </summary>
        Trace = 0,
        /// <summary>
        /// Debug level logging
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Information level logging
        /// </summary>
        Information = 2,

        /// <summary>
        /// Warning level logging
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Error level logging
        /// </summary>
        Error = 4,

        /// <summary>
        /// Critical level logging
        /// </summary>
        Critical = 5
    }
}
