using System.IO;
using Amazon.Runtime;

namespace Milochau.Core.Aws.Lambda.Model
{
    /// <summary>
    /// This is the response object from the Invoke operation.
    /// </summary>
    public partial class InvokeResponse : AmazonWebServiceResponse
    {

        /// <summary>
        /// Gets and sets the property ExecutedVersion. 
        /// <para>
        /// The version of the function that executed. When you invoke a function with an alias,
        /// this indicates which version the alias resolved to.
        /// </para>
        /// </summary>
        public string? ExecutedVersion { get; set; }

        /// <summary>
        /// Gets and sets the property FunctionError. 
        /// <para>
        /// If present, indicates that an error occurred during function execution. Details about
        /// the error are included in the response payload.
        /// </para>
        /// </summary>
        public string? FunctionError { get; set; }

        /// <summary>
        /// Gets and sets the property LogResult. 
        /// <para>
        /// The last 4 KB of the execution log, which is base64-encoded.
        /// </para>
        /// </summary>
        public string? LogResult { get; set; }

        /// <summary>
        /// Gets and sets the property Payload. 
        /// <para>
        /// The response from the function, or an error object.
        /// </para>
        /// </summary>
        public MemoryStream? Payload { get; set; }

        /// <summary>
        /// Gets and sets the property StatusCode. 
        /// <para>
        /// The HTTP status code is in the 200 range for a successful request. For the <code>RequestResponse</code>
        /// invocation type, this status code is 200. For the <code>Event</code> invocation type,
        /// this status code is 202. For the <code>DryRun</code> invocation type, the status code
        /// is 204.
        /// </para>
        /// </summary>
        public int? StatusCode { get; set; }
    }
}