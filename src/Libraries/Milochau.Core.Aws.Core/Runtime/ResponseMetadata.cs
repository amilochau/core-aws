using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Information about the request.
    /// </summary>
    public class ResponseMetadata
    {
        private IDictionary<string, string>? _metadata;

        /// <summary>
        /// Gets and sets the RequestId property.
        /// ID that uniquely identifies a request. Amazon keeps track of request IDs. If you have a question about a request, include the request ID in your correspondence.
        /// </summary>
        public required string RequestId { get; set; }

        public IDictionary<string, string> Metadata
        {
            get
            {
                _metadata ??= new Dictionary<string, string>();

                return _metadata;
            }
        }
    }
}
