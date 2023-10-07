namespace Amazon.Lambda.SNSEvents
{
    using System.Collections.Generic;

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
        public IList<SNSRecord> Records { get; set; }

        /// <summary>
        /// An SNS message record.
        /// </summary>
        public class SNSRecord
        {
            /// <summary>
            /// The SNS message.
            /// </summary>
            public SNSMessage Sns { get; set; }
        }

        /// <summary>
        /// An SNS message record.
        /// </summary>
        public class SNSMessage
        {
            /// <summary>
            /// The message.
            /// </summary>
            public string Message { get; set; }
        }
    }
}
