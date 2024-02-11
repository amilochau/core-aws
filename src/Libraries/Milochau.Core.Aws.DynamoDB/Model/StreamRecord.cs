using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// A description of a single data modification that was performed on an item in a DynamoDB
    /// table.
    /// </summary>
    public class StreamRecord
    {
        /// <summary>
        /// Gets and sets the property ApproximateCreationDateTime. 
        /// <para>
        /// The approximate date and time when the stream record was created, in <a href="http://www.epochconverter.com/">UNIX
        /// epoch time</a> format and rounded down to the closest second.
        /// </para>
        /// </summary>
        public DateTime ApproximateCreationDateTime { get; set; }

        /// <summary>
        /// Gets and sets the property Keys. 
        /// <para>
        /// The primary key attribute(s) for the DynamoDB item that was modified.
        /// </para>
        /// </summary>
        public required Dictionary<string, AttributeValue> Keys { get; set; }

        /// <summary>
        /// Gets and sets the property NewImage. 
        /// <para>
        /// The item in the DynamoDB table as it appeared after it was modified.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? NewImage { get; set; }

        /// <summary>
        /// Gets and sets the property OldImage. 
        /// <para>
        /// The item in the DynamoDB table as it appeared before it was modified.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? OldImage { get; set; }

        /// <summary>
        /// Gets and sets the property SequenceNumber. 
        /// <para>
        /// The sequence number of the stream record.
        /// </para>
        /// </summary>
        public required string SequenceNumber { get; set; }

        /// <summary>
        /// Gets and sets the property SizeBytes. 
        /// <para>
        /// The size of the stream record, in bytes.
        /// </para>
        /// </summary>
        public long SizeBytes { get; set; }

        /// <summary>
        /// Gets and sets the property StreamViewType. 
        /// <para>
        /// The type of data from the modified DynamoDB item that was captured in this stream
        /// record:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>KEYS_ONLY</code> - only the key attributes of the modified item.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>NEW_IMAGE</code> - the entire item, as it appeared after it was modified.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>OLD_IMAGE</code> - the entire item, as it appeared before it was modified.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>NEW_AND_OLD_IMAGES</code> - both the new and the old item images of the item.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public StreamViewType StreamViewType { get; set; }
    }
}
