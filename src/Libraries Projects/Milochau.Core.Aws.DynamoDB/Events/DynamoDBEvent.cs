﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.DynamoDB.Events
{
    /// <summary>
    /// AWS DynamoDB event
    /// http://docs.aws.amazon.com/lambda/latest/dg/with-ddb.html
    /// http://docs.aws.amazon.com/lambda/latest/dg/eventsources.html#eventsources-ddb-update
    /// </summary>
    public class DynamoDBEvent
    {
        /// <summary>
        /// List of DynamoDB event records.
        /// </summary>
        public IList<DynamodbStreamRecord> Records { get; set; }

        /// <summary>
        /// DynamoDB stream record
        /// http://docs.aws.amazon.com/dynamodbstreams/latest/APIReference/API_StreamRecord.html
        /// </summary>
        public class DynamodbStreamRecord : Record
        {
            /// <summary>
            /// The event source arn of DynamoDB.
            /// </summary>
            public string EventSourceArn { get; set; }
        }
    }
}
