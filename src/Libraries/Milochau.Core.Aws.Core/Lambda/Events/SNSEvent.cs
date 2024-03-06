using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.Core.Lambda.Events
{
    /// <summary>
    /// Simple Notification Service event
    /// http://docs.aws.amazon.com/lambda/latest/dg/with-sns.html
    /// http://docs.aws.amazon.com/lambda/latest/dg/eventsources.html#eventsources-sns
    /// </summary>
    public class SNSEvent
    {
        /// <summary>
        /// List of SNS records.
        /// </summary>
        public required IList<SNSRecord> Records { get; set; }
    }

    /// <summary>
    /// An SNS message record.
    /// </summary>
    public class SNSRecord
    {
        /// <summary>
        /// The SNS message.
        /// </summary>
        public required SNSMessage Sns { get; set; }
    }

    /// <summary>
    /// An SNS message record.
    /// </summary>
    public class SNSMessage
    {
        /// <summary>
        /// The subject for the message.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// The message.
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// The message time stamp.
        /// </summary>
        public required DateTime Timestamp { get; set; }
    }
}
