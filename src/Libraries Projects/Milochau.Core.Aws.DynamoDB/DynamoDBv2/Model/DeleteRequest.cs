using System.Collections.Generic;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/DeleteRequest.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model
{
    /// <summary>
    /// Represents a request to perform a <code>DeleteItem</code> operation on an item.
    /// </summary>
    public partial class DeleteRequest
    {
        /// <summary>
        /// Gets and sets the property Key. 
        /// <para>
        /// A map of attribute name to attribute values, representing the primary key of the item
        /// to delete. All of the table's primary key attributes must be specified, and their
        /// data types must match those of the table's key schema.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue> Key { get; set; } = new Dictionary<string, AttributeValue>();

        // Check to see if Key property is set
        internal bool IsSetKey()
        {
            return Key != null && Key.Count > 0;
        }
    }
}
