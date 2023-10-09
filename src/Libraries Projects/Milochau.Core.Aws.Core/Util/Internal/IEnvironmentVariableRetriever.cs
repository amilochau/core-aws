namespace Milochau.Core.Aws.Core.Util.Internal
{
    /// <summary>
    /// Interface for EnvironmentVariableRetriever.
    /// This serves as a property for the singleton EnvironmentVariableSource
    /// </summary>
    public interface IEnvironmentVariableRetriever
    {
        string GetEnvironmentVariable(string key);
    }
}