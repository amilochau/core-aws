using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Util.Internal
{
    /// <summary>
    /// Wrapper class over <see cref="File"/> operations.
    /// This change was done for testability.
    /// </summary>
    public interface IFile
    {
        /// <inheritdoc cref="File.Exists"/>
        bool Exists(string path);

        /// <inheritdoc cref="File.ReadAllText(string)"/>
        string ReadAllText(string path);
        /// <inheritdoc cref="File.WriteAllText(string, string)"/>
        void WriteAllText(string path, string contents);

        /// <inheritdoc cref="File.ReadAllText(string)"/>
        Task<string> ReadAllTextAsync(string path, CancellationToken token = default);
        /// <inheritdoc cref="File.WriteAllText(string, string)"/>
        Task WriteAllTextAsync(string path, string contents, CancellationToken token = default);
    }

    /// <inheritdoc cref="IFile"/>
    public class FileRetriever : IFile
    {
        public bool Exists(string path) => File.Exists(path);

        public string ReadAllText(string path) => File.ReadAllText(path);
        public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);

        public async Task<string> ReadAllTextAsync(string path, CancellationToken token = default)
        {
            using var fs = File.OpenRead(path);
            using var reader = new StreamReader(fs);
            return await reader.ReadToEndAsync().ConfigureAwait(false);

        }

        public async Task WriteAllTextAsync(string path, string contents, CancellationToken token = default)
        {
            //we use  FileMode.Create because we want to truncate the file first if the file exists then write to it.
            using var fs = new FileStream(path, FileMode.Create);
            using var writer = new StreamWriter(fs);
            await writer.WriteAsync(contents).ConfigureAwait(false);

        }
    }
}

