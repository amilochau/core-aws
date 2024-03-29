﻿namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Represents the amount of provisioned throughput capacity consumed on a table or an
    /// index.
    /// </summary>
    public class Capacity
    {
        /// <summary>
        /// Gets and sets the property CapacityUnits. 
        /// <para>
        /// The total number of capacity units consumed on a table or an index.
        /// </para>
        /// </summary>
        public double CapacityUnits { get; set; }

        /// <summary>
        /// Gets and sets the property ReadCapacityUnits. 
        /// <para>
        /// The total number of read capacity units consumed on a table or an index.
        /// </para>
        /// </summary>
        public double ReadCapacityUnits { get; set; }

        /// <summary>
        /// Gets and sets the property WriteCapacityUnits. 
        /// <para>
        /// The total number of write capacity units consumed on a table or an index.
        /// </para>
        /// </summary>
        public double WriteCapacityUnits { get; set; }
    }
}