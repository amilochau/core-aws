namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// An object that defines the entire content of the email, including the message headers
    /// and the body content. You can create a simple email message, in which you specify
    /// the subject and the text and HTML versions of the message body. You can also create
    /// raw messages, in which you specify a complete MIME-formatted message. Raw messages
    /// can include attachments and custom headers.
    /// </summary>
    public partial class EmailContent
    {
        /// <summary>
        /// Gets and sets the property Raw. 
        /// <para>
        /// The raw email message. The message has to meet the following criteria:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// The message has to contain a header and a body, separated by one blank line.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// All of the required header fields must be present in the message.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Each part of a multipart MIME message must be formatted properly.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// If you include attachments, they must be in a file format that the Amazon SES API
        /// v2 supports. 
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// The entire message must be Base64 encoded.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// If any of the MIME parts in your message contain content that is outside of the 7-bit
        /// ASCII character range, you should encode that content to ensure that recipients' email
        /// clients render the message properly.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// The length of any single line of text in the message can't exceed 1,000 characters.
        /// This restriction is defined in <a href="https://tools.ietf.org/html/rfc5321">RFC 5321</a>.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public RawMessage? Raw { get; set; }

        /// <summary>
        /// Gets and sets the property Simple. 
        /// <para>
        /// The simple email message. The message consists of a subject and a message body.
        /// </para>
        /// </summary>
        public Message? Simple { get; set; }

        /// <summary>
        /// Gets and sets the property Template. 
        /// <para>
        /// The template to use for the email message.
        /// </para>
        /// </summary>
        public Template? Template { get; set; }
    }
}