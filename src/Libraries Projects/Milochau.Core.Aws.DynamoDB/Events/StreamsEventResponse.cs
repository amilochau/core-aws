using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Events
{
    /// <summary>
    /// This class is used as the return type for AWS Lambda functions that are invoked by DynamoDB to report batch item failures.
    /// https://docs.aws.amazon.com/lambda/latest/dg/with-ddb.html#services-ddb-batchfailurereporting
    /// </summary>
    public class StreamsEventResponse
    {
        /// <summary>
        /// A list of records which failed processing. Returning the first record which failed would retry all remaining records from the batch.
        /// </summary>
        [JsonPropertyName("batchItemFailures")]
        public IList<BatchItemFailure> BatchItemFailures { get; set; } = new List<BatchItemFailure>();
    }

    /// <summary>
    /// The class representing the BatchItemFailure.
    /// </summary>
    public class BatchItemFailure
    {
        /// <summary>
        /// Sequence number of the record which failed processing.
        /// </summary>
        [JsonPropertyName("itemIdentifier")]
        public string ItemIdentifier { get; set; } = null!;
    }
}
