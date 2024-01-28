﻿using Milochau.Core.Aws.Core.Runtime.Internal;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Base class for DynamoDB operation requests.
    /// </summary>
    public class AmazonDynamoDBRequest : AmazonWebServiceRequest
    {
        /// <summary>
        /// Gets and sets the property ReturnConsumedCapacity.
        /// </summary>
        /// <remarks><see cref="DynamoDB.ReturnConsumedCapacity"/></remarks>
        public string? ReturnConsumedCapacity { get; set; }
    }
}