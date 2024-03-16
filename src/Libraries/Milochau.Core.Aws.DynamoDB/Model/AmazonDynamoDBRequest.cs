﻿using Milochau.Core.Aws.Core.Runtime.Internal;
using System;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Base class for DynamoDB operation requests.
    /// </summary>
    public class AmazonDynamoDBRequest(Guid? userId) : AmazonWebServiceRequest(userId)
    {
        /// <summary>
        /// Gets and sets the property ReturnConsumedCapacity.
        /// </summary>
        public ReturnConsumedCapacity? ReturnConsumedCapacity { get; set; }
    }
}
