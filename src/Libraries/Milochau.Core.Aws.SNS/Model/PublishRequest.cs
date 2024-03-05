using System;
using System.Collections.Generic;

namespace Milochau.Core.Aws.SNS.Model
{
    /// <summary>
    /// Container for the parameters to the Publish operation.
    /// Sends a message to an Amazon SNS topic, a text message (SMS message) directly to a
    /// phone number, or a message to a mobile platform endpoint (when you specify the <c>TargetArn</c>).
    /// 
    ///  
    /// <para>
    /// If you send a message to a topic, Amazon SNS delivers the message to each endpoint
    /// that is subscribed to the topic. The format of the message depends on the notification
    /// protocol for each subscribed endpoint.
    /// </para>
    ///  
    /// <para>
    /// When a <c>messageId</c> is returned, the message is saved and Amazon SNS immediately
    /// delivers it to subscribers.
    /// </para>
    ///  
    /// <para>
    /// To use the <c>Publish</c> action for publishing a message to a mobile endpoint, such
    /// as an app on a Kindle device or mobile phone, you must specify the EndpointArn for
    /// the TargetArn parameter. The EndpointArn is returned when making a call with the <c>CreatePlatformEndpoint</c>
    /// action. 
    /// </para>
    ///  
    /// <para>
    /// For more information about formatting messages, see <a href="https://docs.aws.amazon.com/sns/latest/dg/mobile-push-send-custommessage.html">Send
    /// Custom Platform-Specific Payloads in Messages to Mobile Devices</a>. 
    /// </para>
    ///  <important> 
    /// <para>
    /// You can publish messages only to topics and endpoints in the same Amazon Web Services
    /// Region.
    /// </para>
    ///  </important>
    /// </summary>
    public partial class PublishRequest(Guid? userId) : AmazonSimpleNotificationServiceRequest(userId)
    {
        /// <summary>
        /// Gets and sets the property Message. 
        /// <para>
        /// The message you want to send.
        /// </para>
        ///  
        /// <para>
        /// If you are publishing to a topic and you want to send the same message to all transport
        /// protocols, include the text of the message as a String value. If you want to send
        /// different messages for each transport protocol, set the value of the <c>MessageStructure</c>
        /// parameter to <c>json</c> and use a JSON object for the <c>Message</c> parameter. 
        /// </para>
        ///   
        /// <para>
        /// Constraints:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// With the exception of SMS, messages must be UTF-8 encoded strings and at most 256
        /// KB in size (262,144 bytes, not 262,144 characters).
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// For SMS, each message can contain up to 140 characters. This character limit depends
        /// on the encoding schema. For example, an SMS message can contain 160 GSM characters,
        /// 140 ASCII characters, or 70 UCS-2 characters.
        /// </para>
        ///  
        /// <para>
        /// If you publish a message that exceeds this size limit, Amazon SNS sends the message
        /// as multiple messages, each fitting within the size limit. Messages aren't truncated
        /// mid-word but are cut off at whole-word boundaries.
        /// </para>
        ///  
        /// <para>
        /// The total size limit for a single SMS <c>Publish</c> action is 1,600 characters.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// JSON-specific constraints:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// Keys in the JSON object that correspond to supported transport protocols must have
        /// simple JSON string values.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// The values will be parsed (unescaped) before they are used in outgoing messages.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Outbound notifications are JSON encoded (meaning that the characters will be reescaped
        /// for sending).
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Values have a minimum length of 0 (the empty string, "", is allowed).
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Values have a maximum length bounded by the overall message size (so, including multiple
        /// protocols may limit message sizes).
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Non-string values will cause the key to be ignored.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Keys that do not correspond to supported transport protocols are ignored.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Duplicate keys are not allowed.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Failure to parse or validate any key or value in the message will cause the <c>Publish</c>
        /// call to return an error (no partial delivery).
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Gets and sets the property MessageAttributes. 
        /// <para>
        /// Message attributes for Publish action.
        /// </para>
        /// </summary>
        public Dictionary<string, MessageAttributeValue>? MessageAttributes { get; set; }

        /// <summary>
        /// Gets and sets the property MessageDeduplicationId. 
        /// <para>
        /// This parameter applies only to FIFO (first-in-first-out) topics. The <c>MessageDeduplicationId</c>
        /// can contain up to 128 alphanumeric characters <c>(a-z, A-Z, 0-9)</c> and punctuation
        /// <c>(!"#$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_`{|}~)</c>.
        /// </para>
        ///  
        /// <para>
        /// Every message must have a unique <c>MessageDeduplicationId</c>, which is a token used
        /// for deduplication of sent messages. If a message with a particular <c>MessageDeduplicationId</c>
        /// is sent successfully, any message sent with the same <c>MessageDeduplicationId</c>
        /// during the 5-minute deduplication interval is treated as a duplicate. 
        /// </para>
        ///  
        /// <para>
        /// If the topic has <c>ContentBasedDeduplication</c> set, the system generates a <c>MessageDeduplicationId</c>
        /// based on the contents of the message. Your <c>MessageDeduplicationId</c> overrides
        /// the generated one.
        /// </para>
        /// </summary>
        public string? MessageDeduplicationId { get; set; }

        /// <summary>
        /// Gets and sets the property MessageGroupId. 
        /// <para>
        /// This parameter applies only to FIFO (first-in-first-out) topics. The <c>MessageGroupId</c>
        /// can contain up to 128 alphanumeric characters <c>(a-z, A-Z, 0-9)</c> and punctuation
        /// <c>(!"#$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_`{|}~)</c>.
        /// </para>
        ///  
        /// <para>
        /// The <c>MessageGroupId</c> is a tag that specifies that a message belongs to a specific
        /// message group. Messages that belong to the same message group are processed in a FIFO
        /// manner (however, messages in different message groups might be processed out of order).
        /// Every message must include a <c>MessageGroupId</c>.
        /// </para>
        /// </summary>
        public string? MessageGroupId { get; set; }

        /// <summary>
        /// Gets and sets the property MessageStructure. 
        /// <para>
        /// Set <c>MessageStructure</c> to <c>json</c> if you want to send a different message
        /// for each protocol. For example, using one publish action, you can send a short message
        /// to your SMS subscribers and a longer message to your email subscribers. If you set
        /// <c>MessageStructure</c> to <c>json</c>, the value of the <c>Message</c> parameter
        /// must: 
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// be a syntactically valid JSON object; and
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// contain at least a top-level JSON key of "default" with a value that is a string.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// You can define other top-level keys that define the message you want to send to a
        /// specific transport protocol (e.g., "http").
        /// </para>
        ///  
        /// <para>
        /// Valid value: <c>json</c> 
        /// </para>
        /// </summary>
        public string? MessageStructure { get; set; }

        /// <summary>
        /// Gets and sets the property Subject. 
        /// <para>
        /// Optional parameter to be used as the "Subject" line when the message is delivered
        /// to email endpoints. This field will also be included, if present, in the standard
        /// JSON messages delivered to other endpoints.
        /// </para>
        ///  
        /// <para>
        /// Constraints: Subjects must be ASCII text that begins with a letter, number, or punctuation
        /// mark; must not include line breaks or control characters; and must be less than 100
        /// characters long.
        /// </para>
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets and sets the property TopicArn. 
        /// <para>
        /// The topic you want to publish to.
        /// </para>
        /// </summary>
        public required string TopicArn { get; set; }

        /// <summary>Get request parameters for XRay</summary>
        public override Dictionary<string, object?> GetXRayRequestParameters()
        {
            return new Dictionary<string, object?>
            {
                { "topic_arn", TopicArn },
            };
        }
    }
}
