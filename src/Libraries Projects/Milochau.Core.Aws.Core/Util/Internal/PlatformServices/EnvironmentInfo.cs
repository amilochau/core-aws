namespace Amazon.Util.Internal.PlatformServices
{
    public class EnvironmentInfo 
    {
        public EnvironmentInfo()
        {
            Platform = InternalSDKUtils.DetermineOS();
            PlatformUserAgent = InternalSDKUtils.PlatformUserAgent();
            FrameworkUserAgent = InternalSDKUtils.DetermineFramework();
        }

        public string Platform { get; }

        public string PlatformUserAgent { get; }

        public string FrameworkUserAgent { get; }

        public static readonly EnvironmentInfo Instance = new EnvironmentInfo();
    }
}
