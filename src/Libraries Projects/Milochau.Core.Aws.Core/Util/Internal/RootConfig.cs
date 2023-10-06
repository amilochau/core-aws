namespace Amazon.Util.Internal
{
    /// <summary>
    /// Root AWS config
    /// </summary>
    public partial class RootConfig
    {
        public LoggingConfig Logging { get; private set; }
        public ProxyConfig Proxy { get; private set; }

        public bool CorrectForClockSkew { get; set; }

        public bool UseAlternateUserAgentHeader { get; set; }

        public RootConfig()
        {
            Logging = new LoggingConfig();
            Proxy = new ProxyConfig();

            CorrectForClockSkew = true;
        }
    }
}
