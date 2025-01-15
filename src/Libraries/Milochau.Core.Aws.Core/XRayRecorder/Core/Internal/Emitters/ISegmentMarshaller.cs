using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters
{
    /// <summary>
    /// Interface to marshall segment
    /// </summary>
    public interface ISegmentMarshaller
    {
        /// <summary>
        /// Marshalls the segment into a byte[]
        /// </summary>
        /// <param name="segment">The segment to marshall</param>
        /// <returns>The result byte array</returns>
        string Marshall(Entity segment);
    }
}
