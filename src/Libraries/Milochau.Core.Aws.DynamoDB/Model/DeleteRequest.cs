using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents a request to perform a <c>DeleteItem</c> operation on an item.
    /// </summary>
    public class DeleteRequest
    {
        /// <summary>
        /// Gets and sets the property Key. 
        /// <para>
        /// A map of attribute name to attribute values, representing the primary key of the item
        /// to delete. All of the table's primary key attributes must be specified, and their
        /// data types must match those of the table's key schema.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Key { get; set; }
    }
}
