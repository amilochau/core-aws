using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// Container for the parameters to the SendEmail operation.
    /// Sends an email message. You can use the Amazon SES API v2 to send the following types
    /// of messages:
    /// 
    ///  <ul> <li> 
    /// <para>
    ///  <b>Simple</b> – A standard email message. When you create this type of message, you
    /// specify the sender, the recipient, and the message body, and Amazon SES assembles
    /// the message for you.
    /// </para>
    ///  </li> <li> 
    /// <para>
    ///  <b>Raw</b> – A raw, MIME-formatted email message. When you send this type of email,
    /// you have to specify all of the message headers, as well as the message body. You can
    /// use this message type to send messages that contain attachments. The message that
    /// you specify has to be a valid MIME message.
    /// </para>
    ///  </li> <li> 
    /// <para>
    ///  <b>Templated</b> – A message that contains personalization tags. When you send this
    /// type of email, Amazon SES API v2 automatically replaces the tags with values that
    /// you specify.
    /// </para>
    ///  </li> </ul>
    /// </summary>
    public partial class SendEmailRequest(Guid? userId) : AmazonSimpleEmailServiceV2Request(userId)
    {
        /// <summary>
        /// Gets and sets the property ConfigurationSetName. 
        /// <para>
        /// The name of the configuration set to use when sending the email.
        /// </para>
        /// </summary>
        public string? ConfigurationSetName { get; set; }

        /// <summary>
        /// Gets and sets the property Content. 
        /// <para>
        /// An object that contains the body of the message. You can send either a Simple message
        /// Raw message or a template Message.
        /// </para>
        /// </summary>
        public EmailContent? Content { get; set; }

        /// <summary>
        /// Gets and sets the property Destination. 
        /// <para>
        /// An object that contains the recipients of the email message.
        /// </para>
        /// </summary>
        public Destination? Destination { get; set; }

        /// <summary>
        /// Gets and sets the property EmailTags. 
        /// <para>
        /// A list of tags, in the form of name/value pairs, to apply to an email that you send
        /// using the <c>SendEmail</c> operation. Tags correspond to characteristics of
        /// the email that you define, so that you can publish email sending events. 
        /// </para>
        /// </summary>
        public List<MessageTag>? EmailTags { get; set; }

        /// <summary>
        /// Gets and sets the property FeedbackForwardingEmailAddress. 
        /// <para>
        /// The address that you want bounce and complaint notifications to be sent to.
        /// </para>
        /// </summary>
        public string? FeedbackForwardingEmailAddress { get; set; }

        /// <summary>
        /// Gets and sets the property FeedbackForwardingEmailAddressIdentityArn. 
        /// <para>
        /// This parameter is used only for sending authorization. It is the ARN of the identity
        /// that is associated with the sending authorization policy that permits you to use the
        /// email address specified in the <c>FeedbackForwardingEmailAddress</c> parameter.
        /// </para>
        ///  
        /// <para>
        /// For example, if the owner of example.com (which has ARN arn:aws:ses:us-east-1:123456789012:identity/example.com)
        /// attaches a policy to it that authorizes you to use feedback@example.com, then you
        /// would specify the <c>FeedbackForwardingEmailAddressIdentityArn</c> to be arn:aws:ses:us-east-1:123456789012:identity/example.com,
        /// and the <c>FeedbackForwardingEmailAddress</c> to be feedback@example.com.
        /// </para>
        ///  
        /// <para>
        /// For more information about sending authorization, see the <a href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/sending-authorization.html">Amazon
        /// SES Developer Guide</a>.
        /// </para>
        /// </summary>
        public string? FeedbackForwardingEmailAddressIdentityArn { get; set; }

        /// <summary>
        /// Gets and sets the property FromEmailAddress. 
        /// <para>
        /// The email address to use as the "From" address for the email. The address that you
        /// specify has to be verified. 
        /// </para>
        /// </summary>
        public string? FromEmailAddress { get; set; }

        /// <summary>
        /// Gets and sets the property FromEmailAddressIdentityArn. 
        /// <para>
        /// This parameter is used only for sending authorization. It is the ARN of the identity
        /// that is associated with the sending authorization policy that permits you to use the
        /// email address specified in the <c>FromEmailAddress</c> parameter.
        /// </para>
        ///  
        /// <para>
        /// For example, if the owner of example.com (which has ARN arn:aws:ses:us-east-1:123456789012:identity/example.com)
        /// attaches a policy to it that authorizes you to use sender@example.com, then you would
        /// specify the <c>FromEmailAddressIdentityArn</c> to be arn:aws:ses:us-east-1:123456789012:identity/example.com,
        /// and the <c>FromEmailAddress</c> to be sender@example.com.
        /// </para>
        ///  
        /// <para>
        /// For more information about sending authorization, see the <a href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/sending-authorization.html">Amazon
        /// SES Developer Guide</a>.
        /// </para>
        ///  
        /// <para>
        /// For Raw emails, the <c>FromEmailAddressIdentityArn</c> value overrides the X-SES-SOURCE-ARN
        /// and X-SES-FROM-ARN headers specified in raw email message content.
        /// </para>
        /// </summary>
        public string? FromEmailAddressIdentityArn { get; set; }

        /// <summary>
        /// Gets and sets the property ListManagementOptions. 
        /// <para>
        /// An object used to specify a list or topic to which an email belongs, which will be
        /// used when a contact chooses to unsubscribe.
        /// </para>
        /// </summary>
        public ListManagementOptions? ListManagementOptions { get; set; }

        /// <summary>
        /// Gets and sets the property ReplyToAddresses. 
        /// <para>
        /// The "Reply-to" email addresses for the message. When the recipient replies to the
        /// message, each Reply-to address receives the reply.
        /// </para>
        /// </summary>
        public List<string>? ReplyToAddresses { get; set; }
    }
}