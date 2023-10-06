namespace Amazon.Util.Internal
{
    /// <summary>
    /// Root AWS config
    /// </summary>
    public partial class RootConfig
    {
        public bool CorrectForClockSkew { get; set; }

        public RootConfig()
        {
            CorrectForClockSkew = true;
        }
    }
}
