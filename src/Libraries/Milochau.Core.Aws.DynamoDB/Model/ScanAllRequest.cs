using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>Container for the parameters to the ScanAll operation.</summary>
    public class ScanAllRequest<TEntity>
        where TEntity : class, IDynamoDbScanableEntity<TEntity>
    {
        /// <inheritdoc cref="AmazonWebServiceRequest.UserId"/>
        public required Guid? UserId { get; set; }

        /// <inheritdoc cref="AmazonDynamoDBRequest.ReturnConsumedCapacity"/>
        public ReturnConsumedCapacity? ReturnConsumedCapacity { get; set; }

        /// <inheritdoc cref="ScanRequest{TEntity}.Filters"/>
        public IFilterExpression? Filters { get; set; }

        /// <inheritdoc cref="ScanRequest{TEntity}.ConsistentRead"/>
        public bool ConsistentRead { get; set; }

        /// <inheritdoc cref="ScanRequest{TEntity}.Segment"/>
        public int? Segment { get; set; }

        /// <inheritdoc cref="ScanRequest{TEntity}.TotalSegments"/>
        public int? TotalSegments { get; set; }
    }
}
