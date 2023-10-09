using Amazon.Runtime;
using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of a <code>GetItem</code> operation.
    /// </summary>
    public partial class GetItemResponse : AmazonDynamoDBResponse
    {
        /// <summary>
        /// Gets and sets the property Item. 
        /// <para>
        /// A map of attribute names to <code>AttributeValue</code> objects, as specified by <code>ProjectionExpression</code>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Item { get; set; }
    }
}