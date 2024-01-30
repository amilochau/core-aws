using Milochau.Core.Aws.DynamoDB.Model;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Events
{
    /// <summary>
    /// AWS DynamoDB event
    /// http://docs.aws.amazon.com/lambda/latest/dg/with-ddb.html
    /// http://docs.aws.amazon.com/lambda/latest/dg/eventsources.html#eventsources-ddb-update
    /// </summary>
    public class DynamoDBEvent
    {
        /// <summary>
        /// List of DynamoDB event records.
        /// </summary>
        public required IList<DynamodbStreamRecord> Records { get; set; }

        /// <summary>
        /// DynamoDB stream record
        /// http://docs.aws.amazon.com/dynamodbstreams/latest/APIReference/API_StreamRecord.html
        /// </summary>
        public class DynamodbStreamRecord : Record
        {
            /// <summary>
            /// The event source arn of DynamoDB.
            /// </summary>
            [JsonPropertyName("eventSourceArn")]
            public required string EventSourceArn { get; set; }
        }
    }
}
