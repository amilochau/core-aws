using System.IO;

namespace Milochau.Core.Aws.Core.Util.Internal
{
    /// <summary>
    /// Wrapper class over <see cref="Directory"/> operations.
    /// This change was done for testability.
    /// </summary>
    public interface IDirectory
    {
        /// <inheritdoc cref="Directory.CreateDirectory"/>
        DirectoryInfo CreateDirectory(string path);
    }

    /// <inheritdoc cref="IDirectory"/>
    public class DirectoryRetriever : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);
    }
}