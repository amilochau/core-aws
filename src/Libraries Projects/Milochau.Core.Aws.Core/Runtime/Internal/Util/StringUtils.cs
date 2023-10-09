using System;
using System.IO;

namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    /// <summary>
    /// Utilities for converting objects to strings. Used by the marshaller classes.
    /// </summary>
    public static class StringUtils
    {
        public static string FromMemoryStream(MemoryStream value)
        {
            return Convert.ToBase64String(value.ToArray());
        }
    }
}
