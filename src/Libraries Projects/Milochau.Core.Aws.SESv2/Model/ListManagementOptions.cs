namespace Milochau.Core.Aws.SESv2.Model
{
    /// <summary>
    /// An object used to specify a list or topic to which an email belongs, which will be
    /// used when a contact chooses to unsubscribe.
    /// </summary>
    public partial class ListManagementOptions
    {

        /// <summary>
        /// Gets and sets the property ContactListName. 
        /// <para>
        /// The name of the contact list.
        /// </para>
        /// </summary>
        public string? ContactListName { get; set; }

        // Check to see if ContactListName property is set
        internal bool IsSetContactListName()
        {
            return this.ContactListName != null;
        }

        /// <summary>
        /// Gets and sets the property TopicName. 
        /// <para>
        /// The name of the topic.
        /// </para>
        /// </summary>
        public string? TopicName { get; set; }

        // Check to see if TopicName property is set
        internal bool IsSetTopicName()
        {
            return this.TopicName != null;
        }
    }
}