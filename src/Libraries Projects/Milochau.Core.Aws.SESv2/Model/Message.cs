namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// Represents the email message that you're sending. The <code>Message</code> object
    /// consists of a subject line and a message body.
    /// </summary>
    public partial class Message
    {

        /// <summary>
        /// Gets and sets the property Body. 
        /// <para>
        /// The body of the message. You can specify an HTML version of the message, a text-only
        /// version of the message, or both.
        /// </para>
        /// </summary>
        public Body? Body { get; set; }

        // Check to see if Body property is set
        internal bool IsSetBody()
        {
            return this.Body != null;
        }

        /// <summary>
        /// Gets and sets the property Subject. 
        /// <para>
        /// The subject line of the email. The subject line can only contain 7-bit ASCII characters.
        /// However, you can specify non-ASCII characters in the subject line by using encoded-word
        /// syntax, as described in <a href="https://tools.ietf.org/html/rfc2047">RFC 2047</a>.
        /// </para>
        /// </summary>
        public Content? Subject { get; set; }

        // Check to see if Subject property is set
        internal bool IsSetSubject()
        {
            return this.Subject != null;
        }
    }
}