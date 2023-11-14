namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// Contains the name and value of a tag that you apply to an email. You can use message
    /// tags when you publish email sending events.
    /// </summary>
    public partial class MessageTag
    {
        /// <summary>
        /// Gets and sets the property Name. 
        /// <para>
        /// The name of the message tag. The message tag name has to meet the following criteria:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// It can only contain ASCII letters (a–z, A–Z), numbers (0–9), underscores (_), or dashes
        /// (-).
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// It can contain no more than 256 characters.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets and sets the property Value. 
        /// <para>
        /// The value of the message tag. The message tag value has to meet the following criteria:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// It can only contain ASCII letters (a–z, A–Z), numbers (0–9), underscores (_), or dashes
        /// (-).
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// It can contain no more than 256 characters.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public string? Value { get; set; }
    }
}