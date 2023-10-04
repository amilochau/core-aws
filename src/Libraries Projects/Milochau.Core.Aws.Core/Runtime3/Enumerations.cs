using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Algorithms for validating request and response integrity for supported operations.
    /// These are the algorithms support by the .NET SDK, a given service may 
    /// only use a subset of these.
    /// </summary>
    public enum CoreChecksumAlgorithm
    {
        NONE,
        CRC32C,
        CRC32,
        SHA256,
        SHA1
    }
}
