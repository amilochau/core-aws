using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// A description of a unique event within a stream.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Gets and sets the property AwsRegion. 
        /// <para>
        /// The region in which the <c>GetRecords</c> request was received.
        /// </para>
        /// </summary>
        [JsonPropertyName("awsRegion")]
        public required string AwsRegion { get; set; }

        /// <summary>
        /// Gets and sets the property Dynamodb. 
        /// <para>
        /// The main body of the stream record, containing all of the DynamoDB-specific fields.
        /// </para>
        /// </summary>
        [JsonPropertyName("dynamodb")]
        public required StreamRecord Dynamodb { get; set; }

        /// <summary>
        /// Gets and sets the property EventID. 
        /// <para>
        /// A globally unique identifier for the event that was recorded in this stream record.
        /// </para>
        /// </summary>
        [JsonPropertyName("eventID")]
        public required string EventID { get; set; }

        /// <summary>
        /// Gets and sets the property EventName. 
        /// <para>
        /// The type of data modification that was performed on the DynamoDB table:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>INSERT</c> - a new item was added to the table.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>MODIFY</c> - one or more of an existing item's attributes were modified.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>REMOVE</c> - the item was deleted from the table
        /// </para>
        ///  </li> </ul>
        /// </summary>
        [JsonPropertyName("eventName")]
        public OperationType EventName { get; set; }

        /// <summary>
        /// Gets and sets the property EventSource. 
        /// <para>
        /// The Amazon Web Services service from which the stream record originated. For DynamoDB
        /// Streams, this is <c>aws:dynamodb</c>.
        /// </para>
        /// </summary>
        [JsonPropertyName("eventSource")]
        public required string EventSource { get; set; }

        /// <summary>
        /// Gets and sets the property EventVersion. 
        /// <para>
        /// The version number of the stream record format. This number is updated whenever the
        /// structure of <c>Record</c> is modified.
        /// </para>
        ///  
        /// <para>
        /// Client applications must not assume that <c>eventVersion</c> will remain at
        /// a particular value, as this number is subject to change at any time. In general, <c>eventVersion</c>
        /// will only increase as the low-level DynamoDB Streams API evolves.
        /// </para>
        /// </summary>
        [JsonPropertyName("eventVersion")]
        public required string EventVersion { get; set; }

        /// <summary>
        /// Gets and sets the property UserIdentity. 
        /// <para>
        /// Items that are deleted by the Time to Live process after expiration have the following
        /// fields: 
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// Records[].userIdentity.type
        /// </para>
        ///  
        /// <para>
        /// "Service"
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Records[].userIdentity.principalId
        /// </para>
        ///  
        /// <para>
        /// "dynamodb.amazonaws.com"
        /// </para>
        ///  </li> </ul>
        /// </summary>
        [JsonPropertyName("userIdentity")]
        public Identity? UserIdentity { get; set; }
    }
}
