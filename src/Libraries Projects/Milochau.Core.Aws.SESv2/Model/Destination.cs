using System.Collections.Generic;

namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// An object that describes the recipients for an email.
    /// 
    ///  <note> 
    /// <para>
    /// Amazon SES does not support the SMTPUTF8 extension, as described in <a href="https://tools.ietf.org/html/rfc6531">RFC6531</a>.
    /// For this reason, the <i>local part</i> of a destination email address (the part of
    /// the email address that precedes the @ sign) may only contain <a href="https://en.wikipedia.org/wiki/Email_address#Local-part">7-bit
    /// ASCII characters</a>. If the <i>domain part</i> of an address (the part after the
    /// @ sign) contains non-ASCII characters, they must be encoded using Punycode, as described
    /// in <a href="https://tools.ietf.org/html/rfc3492.html">RFC3492</a>.
    /// </para>
    ///  </note>
    /// </summary>
    public partial class Destination
    {

        /// <summary>
        /// Gets and sets the property BccAddresses. 
        /// <para>
        /// An array that contains the email addresses of the "BCC" (blind carbon copy) recipients
        /// for the email.
        /// </para>
        /// </summary>
        public List<string>? BccAddresses { get; set; }

        // Check to see if BccAddresses property is set
        internal bool IsSetBccAddresses()
        {
            return this.BccAddresses != null && this.BccAddresses.Count > 0; 
        }

        /// <summary>
        /// Gets and sets the property CcAddresses. 
        /// <para>
        /// An array that contains the email addresses of the "CC" (carbon copy) recipients for
        /// the email.
        /// </para>
        /// </summary>
        public List<string>? CcAddresses { get; set; }

        // Check to see if CcAddresses property is set
        internal bool IsSetCcAddresses()
        {
            return this.CcAddresses != null && this.CcAddresses.Count > 0; 
        }

        /// <summary>
        /// Gets and sets the property ToAddresses. 
        /// <para>
        /// An array that contains the email addresses of the "To" recipients for the email.
        /// </para>
        /// </summary>
        public List<string>? ToAddresses { get; set; }

        // Check to see if ToAddresses property is set
        internal bool IsSetToAddresses()
        {
            return this.ToAddresses != null && this.ToAddresses.Count > 0; 
        }
    }
}