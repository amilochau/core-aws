using System;
using System.Collections.Generic;
using System.Threading;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Utility class that stores clock skew information.
    /// </summary>
    public static class CorrectClockSkew
    {
        private static readonly IDictionary<string, TimeSpan> clockCorrectionDictionary = new Dictionary<string, TimeSpan>();
        private static readonly ReaderWriterLockSlim clockCorrectionDictionaryLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Return clock skew correction value for an endpoint if there is one.
        /// 
        /// AWSConfigs.ManualClockCorrection overrides globally any calculated service endpoint specific
        /// clock correction value.
        /// </summary>
        /// <param name="endpoint">Endpoint should in a form such as "https://s3"</param>
        /// <returns>Clock correction value for an endpoint in TimeSpan.  TimeSpan.Zero if no such clock correction is set.</returns>
        public static TimeSpan GetClockCorrectionForEndpoint(string endpoint)
        {
            TimeSpan span;
            bool hasValue = false;
            clockCorrectionDictionaryLock.EnterReadLock();
            try
            {
                hasValue = clockCorrectionDictionary.TryGetValue(endpoint, out span);
            }
            finally
            {
                clockCorrectionDictionaryLock.ExitReadLock();
            }

            return hasValue ? span : TimeSpan.Zero;
        }

        /// <summary>
        /// Get clock skew corrected UTC now value.  If ManualClockCorrection is set, 
        /// use ManualClockCorrection instead of endpoint specific clock correction value.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static DateTime GetCorrectedUtcNowForEndpoint(string endpoint)
        {
            TimeSpan adjustment = GetClockCorrectionForEndpoint(endpoint);

            return DateTime.UtcNow + adjustment;
        }

        internal static void SetClockCorrectionForEndpoint(string endpoint, TimeSpan correction)
        {
            clockCorrectionDictionaryLock.EnterWriteLock();
            try
            {
                clockCorrectionDictionary[endpoint] = correction;
            }
            finally
            {
                clockCorrectionDictionaryLock.ExitWriteLock();
            }
        }
    }
}
