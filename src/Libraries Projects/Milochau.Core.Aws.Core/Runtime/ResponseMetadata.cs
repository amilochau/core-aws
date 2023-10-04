using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Core/Amazon.Runtime/ResponseMetadata.cs
namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Information about the request.
    /// </summary>
    public class ResponseMetadata
    {
        /// <summary>
        /// Gets and sets the RequestId property.
        /// ID that uniquely identifies a request. Amazon keeps track of request IDs. If you have a question about a request, include the request ID in your correspondence.
        /// </summary>
        public string RequestId { get; set; } = null!;

        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Checksum algorithm that was selected to validate the response's integrity
        /// </summary>
        public CoreChecksumAlgorithm ChecksumAlgorithm { get; set; }

        /// <summary>
        ///Status of checksum validation for this response
        /// </summary>
        public ChecksumValidationStatus ChecksumValidationStatus { get; set; }
    }

    /// <summary>
    /// States for response checksum validation 
    /// </summary>
    public enum ChecksumValidationStatus
    {
        /// <summary>
        /// Set when the SDK did not perform checksum validation.
        /// </summary>
        NOT_VALIDATED,
        /// <summary>
        /// Set when a checksum was selected to be validated, but validation
        /// will not completed until the response stream is fully read. At that point an exception
        /// will be thrown if the checksum is invalid.
        /// </summary>
        PENDING_RESPONSE_READ,
        /// <summary>
        /// The checksum has been validated successfully during response unmarshalling.
        /// </summary>
        SUCCESSFUL,
        /// <summary>
        /// The checksum of the response stream did not match the header sent by the service.
        /// </summary>
        INVALID
    }
}
