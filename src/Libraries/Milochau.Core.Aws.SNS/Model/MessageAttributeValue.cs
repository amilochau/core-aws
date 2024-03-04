namespace Milochau.Core.Aws.SNS.Model
{
    /// <summary>
    /// The user-specified message attribute value. For string data types, the value attribute
    /// has the same restrictions on the content as the message body. For more information,
    /// see <a href="https://docs.aws.amazon.com/sns/latest/api/API_Publish.html">Publish</a>.
    /// 
    ///  
    /// <para>
    /// Name, type, and value must not be empty or null. In addition, the message body should
    /// not be empty or null. All parts of the message attribute, including name, type, and
    /// value, are included in the message size restriction, which is currently 256 KB (262,144
    /// bytes). For more information, see <a href="https://docs.aws.amazon.com/sns/latest/dg/SNSMessageAttributes.html">Amazon
    /// SNS message attributes</a> and <a href="https://docs.aws.amazon.com/sns/latest/dg/sms_publish-to-phone.html">Publishing
    /// to a mobile phone</a> in the <i>Amazon SNS Developer Guide.</i> 
    /// </para>
    /// </summary>
    public partial class MessageAttributeValue
    {
        /// <summary>
        /// Gets and sets the property DataType. 
        /// <para>
        /// Amazon SNS supports the following logical data types: String, String.Array, Number,
        /// and Binary. For more information, see <a href="https://docs.aws.amazon.com/sns/latest/dg/SNSMessageAttributes.html#SNSMessageAttributes.DataTypes">Message
        /// Attribute Data Types</a>.
        /// </para>
        /// </summary>
        public required string DataType { get; set; }

        /// <summary>
        /// Gets and sets the property StringValue. 
        /// <para>
        /// Strings are Unicode with UTF8 binary encoding. For a list of code values, see <a href="https://en.wikipedia.org/wiki/ASCII#ASCII_printable_characters">ASCII
        /// Printable Characters</a>.
        /// </para>
        /// </summary>
        public string? StringValue { get; set; }
    }
}
