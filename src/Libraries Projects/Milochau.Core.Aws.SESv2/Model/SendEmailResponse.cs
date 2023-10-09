using Milochau.Core.Aws.Core.Runtime;

namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// A unique message ID that you receive when an email is accepted for sending.
    /// </summary>
    public partial class SendEmailResponse : AmazonWebServiceResponse
    {
        /// <summary>
        /// Gets and sets the property MessageId. 
        /// <para>
        /// A unique identifier for the message that is generated when the message is accepted.
        /// </para>
        ///  <note> 
        /// <para>
        /// It's possible for Amazon SES to accept a message without sending it. This can happen
        /// when the message that you're trying to send has an attachment contains a virus, or
        /// when you send a templated email that contains invalid personalization content, for
        /// example.
        /// </para>
        ///  </note>
        /// </summary>
        public string? MessageId { get; set; }
    }
}