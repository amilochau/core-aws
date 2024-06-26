﻿using System.Collections.Generic;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Information about item collections, if any, that were affected by the operation. <c>ItemCollectionMetrics</c>
    /// is only returned if the request asked for it. If the table does not have any local
    /// secondary indexes, this information is not returned in the response.
    /// </summary>
    public class ItemCollectionMetrics
    {
        /// <summary>
        /// ItemCollectionKey
        /// <para>
        /// The partition key value of the item collection. This value is the same as the partition
        /// key value of the item.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? ItemCollectionKey { get; set; }

        /// <summary>
        /// SizeEstimateRangeGB
        /// <para>
        /// An estimate of item collection size, in gigabytes. This value is a two-element array
        /// containing a lower bound and an upper bound for the estimate. The estimate includes
        /// the size of all the items in the table, plus the size of all attributes projected
        /// into all of the local secondary indexes on that table. Use this estimate to measure
        /// whether a local secondary index is approaching its size limit.
        /// </para>
        ///  
        /// <para>
        /// The estimate is subject to change over time; therefore, do not rely on the precision
        /// or accuracy of the estimate.
        /// </para>
        /// </summary>
        public List<double>? SizeEstimateRangeGB { get; set; }
    }
}