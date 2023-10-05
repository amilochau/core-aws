namespace Amazon.Util.Internal
{
    /// <summary>
    /// Root AWS config
    /// </summary>
    public partial class RootConfig
    {
        public CSMConfig CSMConfig { get; set; }
        public LoggingConfig Logging { get; private set; }
        public ProxyConfig Proxy { get; private set; }
        public string ProfilesLocation { get; set; }
        public bool UseSdkCache { get; set; }

        public bool CorrectForClockSkew { get; set; }

        public bool UseAlternateUserAgentHeader { get; set; }

        public RootConfig()
        {
            CSMConfig = new CSMConfig();
            Logging = new LoggingConfig();
            Proxy = new ProxyConfig();

            ProfilesLocation = null;
            UseSdkCache = true;
            CorrectForClockSkew = true;
        }
    }
}
