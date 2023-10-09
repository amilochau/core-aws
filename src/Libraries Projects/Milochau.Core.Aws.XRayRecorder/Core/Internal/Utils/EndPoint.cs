using System.Net;

namespace Milochau.Core.Aws.XRayRecorder.Core.Internal.Utils
{
    /// <summary>
    /// Represents either a IPEndPoint or HostEndPoint.
    /// </summary>
    public class EndPoint
    {
        private HostEndPoint? _h;
        private IPEndPoint? _i;

        private EndPoint()
        {
        }

        /// <summary>
        /// Create an EndPoint representing a HostEndPoint.
        /// </summary>
        /// <param name="hostEndPoint">the host endpoint to represent.</param>
        /// <returns></returns>
        public static EndPoint Of(HostEndPoint hostEndPoint)
        {
            return new EndPoint { _h = hostEndPoint };
        }

        /// <summary>
        /// Create an EndPoint representing an IPEndPoint.
        /// </summary>
        /// <param name="ipEndPoint">the ip endpoint to represent.</param>
        /// <returns></returns>
        public static EndPoint Of(IPEndPoint? ipEndPoint)
        {
            return new EndPoint { _i = ipEndPoint };
        }

        /// <summary>
        /// Gets the ip of the endpoint that is represented.
        /// </summary>
        /// <returns></returns>
        public IPEndPoint? GetIPEndPoint()
        {
            return _h != null ? _h.GetIPEndPoint(out _) : _i;
        }
    }
}