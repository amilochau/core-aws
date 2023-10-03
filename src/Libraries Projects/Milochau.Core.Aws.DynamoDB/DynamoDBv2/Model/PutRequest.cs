using System.Collections.Generic;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/PutRequest.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// Represents a request to perform a <code>PutItem</code> operation on an item.
    /// </summary>
    public partial class PutRequest
    {
        /// <summary>
        /// Gets and sets the property Item. 
        /// <para>
        /// A map of attribute name to attribute values, representing the primary key of an item
        /// to be processed by <code>PutItem</code>. All of the table's primary key attributes
        /// must be specified, and their data types must match those of the table's key schema.
        /// If any attributes are present in the item that are part of an index key schema for
        /// the table, their types must match the index key schema.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue> Item { get; set; } = new Dictionary<string, AttributeValue>();

        // Check to see if Item property is set
        internal bool IsSetItem()
        {
            return Item != null && Item.Count > 0;
        }
    }
}
