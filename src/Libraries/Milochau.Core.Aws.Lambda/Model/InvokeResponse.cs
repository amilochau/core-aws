using Milochau.Core.Aws.Core.Runtime.Internal;
using System.Collections.Generic;
using System.IO;

namespace Milochau.Core.Aws.Lambda.Model
{
    /// <summary>
    /// This is the response object from the Invoke operation.
    /// </summary>
    public partial class InvokeResponse : AmazonWebServiceResponse
    {
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
        /// The HTTP status code is in the 200 range for a successful request. For the <c>RequestResponse</c>
        /// invocation type, this status code is 200. For the <c>Event</c> invocation type,
        /// this status code is 202. For the <c>DryRun</c> invocation type, the status code
        /// is 204.
        /// </para>
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>Get response parameters for XRay</summary>
        public override Dictionary<string, object?> GetXRayResponseParameters()
        {
            return new Dictionary<string, object?>
            {
                { "status", StatusCode },
            };
        }
    }
}