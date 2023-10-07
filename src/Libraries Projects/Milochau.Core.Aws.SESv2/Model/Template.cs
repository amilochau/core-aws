namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// An object that defines the email template to use for an email message, and the values
    /// to use for any message variables in that template. An <i>email template</i> is a type
    /// of message template that contains content that you want to define, save, and reuse
    /// in email messages that you send.
    /// </summary>
    public partial class Template
    {

        /// <summary>
        /// Gets and sets the property TemplateArn. 
        /// <para>
        /// The Amazon Resource Name (ARN) of the template.
        /// </para>
        /// </summary>
        public string? TemplateArn { get; set; }

        // Check to see if TemplateArn property is set
        internal bool IsSetTemplateArn()
        {
            return this.TemplateArn != null;
        }

        /// <summary>
        /// Gets and sets the property TemplateData. 
        /// <para>
        /// An object that defines the values to use for message variables in the template. This
        /// object is a set of key-value pairs. Each key defines a message variable in the template.
        /// The corresponding value defines the value to use for that variable.
        /// </para>
        /// </summary>
        public string ?TemplateData { get; set; }

        // Check to see if TemplateData property is set
        internal bool IsSetTemplateData()
        {
            return this.TemplateData != null;
        }

        /// <summary>
        /// Gets and sets the property TemplateName. 
        /// <para>
        /// The name of the template. You will refer to this name when you send email using the
        /// <code>SendTemplatedEmail</code> or <code>SendBulkTemplatedEmail</code> operations.
        /// 
        /// </para>
        /// </summary>
        public string? TemplateName { get; set; }

        // Check to see if TemplateName property is set
        internal bool IsSetTemplateName()
        {
            return this.TemplateName != null;
        }
    }
}