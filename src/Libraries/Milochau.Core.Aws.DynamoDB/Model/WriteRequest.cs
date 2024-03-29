﻿namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents an operation to perform - either <code>DeleteItem</code> or <code>PutItem</code>.
    /// You can only request one of these operations, not both, in a single <code>WriteRequest</code>.
    /// If you do need to perform both of these operations, you need to provide two separate
    /// <code>WriteRequest</code> objects.
    /// </summary>
    public class WriteRequest
    {
        /// <summary>
        /// Gets and sets the property DeleteRequest. 
        /// <para>
        /// A request to perform a <code>DeleteItem</code> operation.
        /// </para>
        /// </summary>
        public DeleteRequest? DeleteRequest { get; set; }

        /// <summary>
        /// Gets and sets the property PutRequest. 
        /// <para>
        /// A request to perform a <code>PutItem</code> operation.
        /// </para>
        /// </summary>
        public PutRequest? PutRequest { get; set; }
    }
}
