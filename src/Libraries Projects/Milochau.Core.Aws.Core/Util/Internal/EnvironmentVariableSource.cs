namespace Milochau.Core.Aws.Core.Util.Internal.Util.Internal
{
    /// <summary>
    /// Singleton class that holds the property of type IEnvironmentVariableRetreiver.
    /// This property can hold an instance of type EnvironmentVariableRetreiver which has a wrapper 
    /// method for 
    /// public static string GetEnvironmentVariable(string variable)
    /// or can be mocked for testing purposes.
    /// </summary>
    public sealed class EnvironmentVariableSource
    {
        private static readonly EnvironmentVariableSource instance = new EnvironmentVariableSource();

        private EnvironmentVariableSource()
        {
        }

        static EnvironmentVariableSource()
        {
        }
        public IEnvironmentVariableRetriever EnvironmentVariableRetriever { get; set; } = new EnvironmentVariableRetriever();

        public static EnvironmentVariableSource Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
