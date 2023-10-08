using Amazon.XRay.Recorder.Core.Internal.Entities;
using System;
using System.Collections.Generic;

namespace Amazon.XRay.Recorder.Core.Strategies
{
    /// <summary>
    /// Interface used to implement custom exception serialization strategy and record <see cref="Exception"/> on <see cref="Cause"/> instance.
    /// </summary>
   public interface ExceptionSerializationStrategy
    {
        /// <summary>
        /// Decribes exception by iterating subsegments and populates list of <see cref="ExceptionDescriptor"/>.
        /// </summary>
        /// <param name="e">The exception to be added</param>
        /// <param name="subsegments">The subsegments to search for existing exception descriptor.</param>
        /// <returns> List of <see cref="ExceptionDescriptor"/></returns>
        List<ExceptionDescriptor> DescribeException(Exception e, IEnumerable<Subsegment> subsegments);
    }
}
