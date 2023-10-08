using System.Net;

namespace Amazon.XRay.Recorder.Core.Internal.Utils
{
    /// <summary>
    /// Represents either a IPEndPoint or HostEndPoint.
    /// </summary>
    public class EndPoint
    {
        private HostEndPoint _h;
        private IPEndPoint _i;
        private bool _isHost;

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
            return new EndPoint {_isHost = true, _h = hostEndPoint};
        }

        /// <summary>
        /// Create an EndPoint representing an IPEndPoint.
        /// </summary>
        /// <param name="ipEndPoint">the ip endpoint to represent.</param>
        /// <returns></returns>
        public static EndPoint Of(IPEndPoint ipEndPoint)
        {
            return new EndPoint {_isHost = false, _i = ipEndPoint};
        }

        /// <summary>
        /// Gets the ip of the endpoint that is represented.
        /// </summary>
        /// <returns></returns>
        public IPEndPoint GetIPEndPoint()
        {
            return _isHost ? _h.GetIPEndPoint(out _) : _i;
        }
    }
}