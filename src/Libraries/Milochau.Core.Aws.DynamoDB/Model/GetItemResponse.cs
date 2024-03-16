using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of a <c>GetItem</c> operation.
    /// </summary>
    public class GetItemResponse : AmazonDynamoDBResponse
    {
        /// <summary>
        /// Gets and sets the property Item. 
        /// <para>
        /// A map of attribute names to <c>AttributeValue</c> objects, as specified by <c>ProjectionExpression</c>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Item { get; set; }
    }
}