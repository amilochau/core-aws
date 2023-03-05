using System.Collections.Generic;

namespace Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context
{
    public interface IClientContext
    {
        /// <summary>
        /// Environment information provided by mobile SDK. 
        /// </summary>
        IDictionary<string, string>? Environment { get; }

        /// <summary>
        /// The client information provided by the AWS Mobile SDK.
        /// </summary>
        IClientApplication? Client { get; }

        /// <summary>
        /// Custom values set by the client application.
        /// </summary>
        IDictionary<string, string>? Custom { get; }
    }

    public interface IClientApplication
    {
        /// <summary>
        /// The application's package name.
        /// </summary>
        string? AppPackageName { get; }

        /// <summary>
        /// The application's title.
        /// </summary>
        string? AppTitle { get; }

        /// <summary>
        /// The application's version code.
        /// </summary>
        string? AppVersionCode { get; }

        /// <summary>
        /// The application's version.
        /// </summary>
        string? AppVersionName { get; }

        /// <summary>
        /// The application's installation id.
        /// </summary>
        string? InstallationId { get; }
    }
}
