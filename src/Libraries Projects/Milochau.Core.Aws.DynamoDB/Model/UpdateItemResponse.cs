﻿using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the output of an <code>UpdateItem</code> operation.
    /// </summary>
    public partial class UpdateItemResponse : AmazonDynamoDBResponse
    {
        /// <summary>
        /// Gets and sets the property Attributes. 
        /// <para>
        /// A map of attribute values as they appear before or after the <code>UpdateItem</code>
        /// operation, as determined by the <code>ReturnValues</code> parameter.
        /// </para>
        ///  
        /// <para>
        /// The <code>Attributes</code> map is only present if the update was successful and <code>ReturnValues</code>
        /// was specified as something other than <code>NONE</code> in the request. Each element
        /// represents one attribute.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? Attributes { get; set; }

        /// <summary>
        /// Gets and sets the property ItemCollectionMetrics. 
        /// <para>
        /// Information about item collections, if any, that were affected by the <code>UpdateItem</code>
        /// operation. <code>ItemCollectionMetrics</code> is only returned if the <code>ReturnItemCollectionMetrics</code>
        /// parameter was specified. If the table does not have any local secondary indexes, this
        /// information is not returned in the response.
        /// </para>
        ///  
        /// <para>
        /// Each <code>ItemCollectionMetrics</code> element consists of:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>ItemCollectionKey</code> - The partition key value of the item collection.
        /// This is the same as the partition key value of the item itself.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>SizeEstimateRangeGB</code> - An estimate of item collection size, in gigabytes.
        /// This value is a two-element array containing a lower bound and an upper bound for
        /// the estimate. The estimate includes the size of all the items in the table, plus the
        /// size of all attributes projected into all of the local secondary indexes on that table.
        /// Use this estimate to measure whether a local secondary index is approaching its size
        /// limit.
        /// </para>
        ///  
        /// <para>
        /// The estimate is subject to change over time; therefore, do not rely on the precision
        /// or accuracy of the estimate.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        public ItemCollectionMetrics? ItemCollectionMetrics { get; set; }
    }
}