using System.IO;

namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// Represents the raw content of an email message.
    /// </summary>
    public partial class RawMessage
    {
        /// <summary>
        /// Gets and sets the property Data. 
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
        /// Attachments must be in a file format that the Amazon SES supports.
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
        public MemoryStream? Data { get; set; }

        // Check to see if Data property is set
        internal bool IsSetData()
        {
            return this.Data != null;
        }
    }
}