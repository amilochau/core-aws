using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies
{
    /// <summary>
    /// Interface used to implement custom exception serialization strategy and record <see cref="Exception"/> on <see cref="Cause"/> instance.
    /// </summary>
   public interface IExceptionSerializationStrategy
    {
        /// <summary>
        /// Decribes exception by iterating subsegments and populates list of <see cref="ExceptionDescriptor"/>.
        /// </summary>
        /// <param name="e">The exception to be added</param>
        /// <returns> List of <see cref="ExceptionDescriptor"/></returns>
        List<ExceptionDescriptor> DescribeException(Exception? e);
    }
}
