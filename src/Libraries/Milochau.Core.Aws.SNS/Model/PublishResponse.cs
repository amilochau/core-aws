using Milochau.Core.Aws.Core.Runtime.Internal;

namespace Milochau.Core.Aws.SNS.Model
{
    /// <summary>
    /// Response for Publish action.
    /// </summary>
    public partial class PublishResponse : AmazonWebServiceResponse
    {
        /// <summary>
        /// Gets and sets the property MessageId. 
        /// <para>
        /// Unique identifier assigned to the published message.
        /// </para>
        ///  
        /// <para>
        /// Length Constraint: Maximum 100 characters
        /// </para>
        /// </summary>
        public string? MessageId { get; set; }

        /// <summary>
        /// Gets and sets the property SequenceNumber. 
        /// <para>
        /// This response element applies only to FIFO (first-in-first-out) topics. 
        /// </para>
        ///  
        /// <para>
        /// The sequence number is a large, non-consecutive number that Amazon SNS assigns to
        /// each message. The length of <c>SequenceNumber</c> is 128 bits. <c>SequenceNumber</c>
        /// continues to increase for each <c>MessageGroupId</c>.
        /// </para>
        /// </summary>
        public string? SequenceNumber { get; set; }
    }
}
