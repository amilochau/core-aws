using System;
using System.Runtime.CompilerServices;

namespace Milochau.Core.Aws.Core.Util.Internal
{
    /// <summary>
    /// Array extensions for cross compilation across different supported framework versions.
    /// </summary>
    static class ArrayEx
    {
        /// <summary>
        /// Returns an empty array.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <returns>An empty array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Empty<T>()
        {
            return Array.Empty<T>();
        }
    }
}