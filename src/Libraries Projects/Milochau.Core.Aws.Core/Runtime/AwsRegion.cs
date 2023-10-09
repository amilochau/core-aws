using Milochau.Core.Aws.Core.RegionEndpoints;
using System;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Base class for determining region based on inspection.
    /// </summary>
    public abstract class AWSRegion
    {
        public RegionEndpoint Region { get; protected set; }

        /// <summary>
        /// Sets the Region property by looking up the corresponding RegionEndpoint
        /// from the supplied region system name (us-east-1, us-west-2 etc).
        /// </summary>
        /// <param name="regionSystemName">The system name of the region.</param>
        protected void SetRegionFromName(string regionSystemName)
        {
            Region = RegionEndpoint.GetBySystemName(regionSystemName);
        }
    }

    /// <summary>
    /// Determines region based on an environment variable. If the environment does not contain
    /// the region setting key an InvalidOperationException is thrown.
    /// </summary>
    public class EnvironmentVariableAWSRegion : AWSRegion
    {
        public const string ENVIRONMENT_VARIABLE_REGION = "AWS_REGION";
        public const string ENVIRONMENT_VARIABLE_DEFAULT_REGION = "AWS_DEFAULT_REGION";

        /// <summary>
        /// Attempts to construct an instance of EnvironmentVariableAWSRegion. If no region is found in the
        /// environment then an InvalidOperationException is thrown.
        /// </summary>
        public EnvironmentVariableAWSRegion()
        {
            string regionName = Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_REGION);
            SetRegionFromName(regionName);
        }
    }

    /// <summary>
    /// Probing mechanism to determine region from various sources.
    /// </summary>
    public static class FallbackRegionFactory
    {
        private static object _lock = new object();

        static FallbackRegionFactory()
        {
            Reset();
        }

        private delegate AWSRegion RegionGenerator();

        public static void Reset()
        {
            cachedRegion = null;
        }

        private static AWSRegion cachedRegion;

        public static RegionEndpoint GetRegionEndpoint()
        {
            lock(_lock)
            {
                if (cachedRegion == null)
                {
                    cachedRegion = new EnvironmentVariableAWSRegion();
                }

                return cachedRegion.Region;
            }
        }
    }
}