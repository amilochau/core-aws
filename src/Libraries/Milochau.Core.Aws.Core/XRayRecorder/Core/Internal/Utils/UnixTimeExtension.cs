using System;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils
{
    /// <summary>
    /// Common utility functions
    /// </summary>
    public static class UnixTimeExtension
    {
        private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;
        private const long MicrosecondPerSecond = TimeSpan.TicksPerSecond / TicksPerMicrosecond;

        private static readonly DateTime _epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly long _unixEpochMicroseconds = _epochStart.Ticks / TicksPerMicrosecond;

        /// <summary>
        /// Convert a given time to Unix time which is the number of seconds
        /// since 1st January 1970, 00:00:00 UTC.
        /// </summary>
        /// <param name="date">.Net representation of time</param>
        /// <returns>The number of seconds elapsed since 1970-01-01 00:00:00 UTC.
        /// The value is expressed in whole and fractional seconds with resolution of microsecond.</returns>
        public static decimal ToUnixTimeSeconds(this DateTime date)
        {
            long microseconds = date.Ticks / TicksPerMicrosecond;
            long microsecondsSinceEpoch = microseconds - _unixEpochMicroseconds;
            return (decimal)microsecondsSinceEpoch / MicrosecondPerSecond;
        }
    }
}
