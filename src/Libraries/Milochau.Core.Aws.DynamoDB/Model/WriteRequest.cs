namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents an operation to perform - either <c>DeleteItem</c> or <c>PutItem</c>.
    /// You can only request one of these operations, not both, in a single <c>WriteRequest</c>.
    /// If you do need to perform both of these operations, you need to provide two separate
    /// <c>WriteRequest</c> objects.
    /// </summary>
    public class WriteRequest
    {
        /// <summary>
        /// Gets and sets the property DeleteRequest. 
        /// <para>
        /// A request to perform a <c>DeleteItem</c> operation.
        /// </para>
        /// </summary>
        public DeleteRequest? DeleteRequest { get; set; }

        /// <summary>
        /// Gets and sets the property PutRequest. 
        /// <para>
        /// A request to perform a <c>PutItem</c> operation.
        /// </para>
        /// </summary>
        public PutRequest? PutRequest { get; set; }
    }
}
