using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils;
using System;
using System.Threading;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling.Local
{
    /// <summary>
    /// The RateLimiter will distribute permit to the first <see cref="LimitPerSecond"/> requests arrives in every epoch second, and block any request that comes later in that second.
    /// </summary>
    /// <param name="limitPerSecond">The number of request that will be permitted every epoch second.</param>
    public class RateLimiter(long limitPerSecond)
    {
        private long countInLastSecond;
        private long lastSecond;

        /// <summary>
        /// Gets or sets the limit per second.
        /// </summary>
        public long LimitPerSecond { get; set; } = limitPerSecond;

        /// <summary>
        /// Request a single permit from this <see cref="RateLimiter"/>.
        /// </summary>
        /// <returns>A value that indicates whether a permit is successfully acquired.</returns>
        public bool Request()
        {
            long now = (long)decimal.Floor(DateTime.UtcNow.ToUnixTimeSeconds());
            if (now != lastSecond)
            {
                Interlocked.Exchange(ref countInLastSecond, 0);
                lastSecond = now;
            }

            if (Interlocked.Read(ref countInLastSecond) < LimitPerSecond)
            {
                // In edge case, the count may go above limit, as the increment and read together is not atomic.
                // The potential overrun will be fairly small because the read and increment is close. It is not
                // worth solving at the cost of performance.
                Interlocked.Increment(ref countInLastSecond);
                return true;
            }

            return false;
        }
    }
}
