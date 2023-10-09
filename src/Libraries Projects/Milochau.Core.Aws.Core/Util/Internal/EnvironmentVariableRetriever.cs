using System;

namespace Milochau.Core.Aws.Core.Util.Internal
{
    /// <summary>
    /// Wrapper class which invokes the static method 
    /// public static string GetEnvironmentVariable(string variable)
    /// underneath. This class is added as a property on the singleton class
    /// EnvironmentVariableSource. This change was done for testability.
    /// </summary>
    public sealed class EnvironmentVariableRetriever : IEnvironmentVariableRetriever
    {
        public string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
