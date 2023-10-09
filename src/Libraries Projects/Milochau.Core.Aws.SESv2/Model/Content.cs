namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// An object that represents the content of the email, and optionally a character set
    /// specification.
    /// </summary>
    public partial class Content
    {

        /// <summary>
        /// Gets and sets the property Charset. 
        /// <para>
        /// The character set for the content. Because of the constraints of the SMTP protocol,
        /// Amazon SES uses 7-bit ASCII by default. If the text includes characters outside of
        /// the ASCII range, you have to specify a character set. For example, you could specify
        /// <code>UTF-8</code>, <code>ISO-8859-1</code>, or <code>Shift_JIS</code>.
        /// </para>
        /// </summary>
        public string? Charset { get; set; }

        // Check to see if Charset property is set
        internal bool IsSetCharset()
        {
            return Charset != null;
        }

        /// <summary>
        /// Gets and sets the property Data. 
        /// <para>
        /// The content of the message itself.
        /// </para>
        /// </summary>
        public string? Data { get; set; }

        // Check to see if Data property is set
        internal bool IsSetData()
        {
            return Data != null;
        }
    }
}