namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// Represents the body of the email message.
    /// </summary>
    public partial class Body
    {

        /// <summary>
        /// Gets and sets the property Html. 
        /// <para>
        /// An object that represents the version of the message that is displayed in email clients
        /// that support HTML. HTML messages can include formatted text, hyperlinks, images, and
        /// more. 
        /// </para>
        /// </summary>
        public Content? Html { get; set; }

        // Check to see if Html property is set
        internal bool IsSetHtml()
        {
            return Html != null;
        }

        /// <summary>
        /// Gets and sets the property Text. 
        /// <para>
        /// An object that represents the version of the message that is displayed in email clients
        /// that don't support HTML, or clients where the recipient has disabled HTML rendering.
        /// </para>
        /// </summary>
        public Content? Text { get; set; }

        // Check to see if Text property is set
        internal bool IsSetText()
        {
            return Text != null;
        }
    }
}