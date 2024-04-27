using Milochau.Core.Aws.Core.Helpers;
using Milochau.Core.Aws.Core.Runtime.Internal;
using Milochau.Core.Aws.DynamoDB.Helpers;
using Milochau.Core.Aws.DynamoDB.Model.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.DynamoDB.Model
{
    /// <summary>
    /// Container for the parameters to the Scan operation.
    /// The <c>Scan</c> operation returns one or more items and item attributes by accessing
    /// every item in a table or a secondary index. To have DynamoDB return fewer items, you
    /// can provide a <c>FilterExpression</c> operation.
    /// <para>
    /// If the total size of scanned items exceeds the maximum dataset size limit of 1 MB,
    /// the scan completes and results are returned to the user. The <c>LastEvaluatedKey</c>
    /// value is also returned and the requestor can use the <c>LastEvaluatedKey</c> to continue
    /// the scan in a subsequent operation. Each scan response also includes number of items
    /// that were scanned (ScannedCount) as part of the request. If using a <c>FilterExpression</c>,
    /// a scan result can result in no items meeting the criteria and the <c>Count</c> will
    /// result in zero. If you did not use a <c>FilterExpression</c> in the scan request,
    /// then <c>Count</c> is the same as <c>ScannedCount</c>.
    /// </para>
    /// <para>
    ///  <c>Count</c> and <c>ScannedCount</c> only return the count of items specific to a
    /// single scan request and, unless the table is less than 1MB, do not represent the total
    /// number of items in the table. 
    /// </para>
    /// <para>
    /// A single <c>Scan</c> operation first reads up to the maximum number of items set (if
    /// using the <c>Limit</c> parameter) or a maximum of 1 MB of data and then applies any
    /// filtering to the results if a <c>FilterExpression</c> is provided. If <c>LastEvaluatedKey</c>
    /// is present in the response, pagination is required to complete the full table scan.
    /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Scan.html#Scan.Pagination">Paginating
    /// the Results</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </para>
    /// <para>
    ///  <c>Scan</c> operations proceed sequentially; however, for faster performance on a
    /// large table or secondary index, applications can request a parallel <c>Scan</c> operation
    /// by providing the <c>Segment</c> and <c>TotalSegments</c> parameters. For more information,
    /// see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Scan.html#Scan.ParallelScan">Parallel
    /// Scan</a> in the <i>Amazon DynamoDB Developer Guide</i>.
    /// </para>
    /// <para>
    /// By default, a <c>Scan</c> uses eventually consistent reads when accessing the items
    /// in a table. Therefore, the results from an eventually consistent <c>Scan</c> may not
    /// include the latest item changes at the time the scan iterates through each item in
    /// the table. If you require a strongly consistent read of each item as the scan iterates
    /// through the items in the table, you can set the <c>ConsistentRead</c> parameter to
    /// true. Strong consistency only relates to the consistency of the read at the item level.
    /// </para>
    /// <para>
    ///  DynamoDB does not provide snapshot isolation for a scan operation when the <c>ConsistentRead</c>
    /// parameter is set to true. Thus, a DynamoDB scan operation does not guarantee that
    /// all reads in a scan see a consistent snapshot of the table when the scan operation
    /// was requested. 
    /// </para>
    /// </summary>
    public partial class ScanRequest(Guid? userId) : AmazonDynamoDBRequest(userId)
    {
        /// <summary>
        /// Table name
        /// <para>
        /// The name of the table containing the requested items or if you provide <c>IndexName</c>,
        /// the name of the table to which that index belongs.
        /// </para>
        /// <para>
        /// You can also provide the Amazon Resource Name (ARN) of the table in this parameter.
        /// </para>
        /// </summary>
        public required string TableName { get; set; }

        /// <summary>
        /// Index name
        /// <para>
        /// The name of a secondary index to scan. This index can be any local secondary index
        /// or global secondary index. Note that if you use the <c>IndexName</c> parameter, you
        /// must also provide <c>TableName</c>.
        /// </para>
        /// </summary>
        public string? IndexName { get; set; }

        /// <summary>
        /// Consistent read
        /// <para>
        /// A Boolean value that determines the read consistency model during the scan:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// If <c>ConsistentRead</c> is <c>false</c>, then the data returned from <c>Scan</c>
        /// might not contain the results from other recently completed write operations (<c>PutItem</c>,
        /// <c>UpdateItem</c>, or <c>DeleteItem</c>).
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// If <c>ConsistentRead</c> is <c>true</c>, then all of the write operations that completed
        /// before the <c>Scan</c> began are guaranteed to be contained in the <c>Scan</c> response.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// The default setting for <c>ConsistentRead</c> is <c>false</c>.
        /// </para>
        ///  
        /// <para>
        /// The <c>ConsistentRead</c> parameter is not supported on global secondary indexes.
        /// If you scan a global secondary index with <c>ConsistentRead</c> set to true, you will
        /// receive a <c>ValidationException</c>.
        /// </para>
        /// </summary>
        public bool? ConsistentRead { get; set; }

        /// <summary>
        /// Exclusive start key
        /// <para>
        /// The primary key of the first item that this operation will evaluate. Use the value
        /// that was returned for <c>LastEvaluatedKey</c> in the previous operation.
        /// </para>
        /// <para>
        /// The data type for <c>ExclusiveStartKey</c> must be String, Number or Binary. No set
        /// data types are allowed.
        /// </para>
        /// <para>
        /// In a parallel scan, a <c>Scan</c> request that includes <c>ExclusiveStartKey</c> must
        /// specify the same segment whose previous <c>Scan</c> returned the corresponding value
        /// of <c>LastEvaluatedKey</c>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? ExclusiveStartKey { get; set; }

        /// <summary>
        /// Expression attribute names
        /// <para>
        /// One or more substitution tokens for attribute names in an expression. The following
        /// are some use cases for using <c>ExpressionAttributeNames</c>:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// To access an attribute whose name conflicts with a DynamoDB reserved word.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// To create a placeholder for repeating occurrences of an attribute name in an expression.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// To prevent special characters in an attribute name from being misinterpreted in an
        /// expression.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// Use the <b>#</b> character in an expression to dereference an attribute name. For
        /// example, consider the following attribute name:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>Percentile</c> 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// The name of this attribute conflicts with a reserved word, so it cannot be used directly
        /// in an expression. (For the complete list of reserved words, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ReservedWords.html">Reserved
        /// Words</a> in the <i>Amazon DynamoDB Developer Guide</i>). To work around this, you
        /// could specify the following for <c>ExpressionAttributeNames</c>:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>{"#P":"Percentile"}</c> 
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// You could then use this substitution in an expression, as in this example:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>#P = :val</c> 
        /// </para>
        ///  </li> </ul>
        /// <para>
        /// Tokens that begin with the <b>:</b> character are <i>expression attribute values</i>,
        /// which are placeholders for the actual value at runtime.
        /// </para>
        /// <para>
        /// For more information on expression attribute names, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.AccessingItemAttributes.html">Specifying
        /// Item Attributes</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, string>? ExpressionAttributeNames { get; set; }

        /// <summary>
        /// Expression attribute values
        /// <para>
        /// One or more values that can be substituted in an expression.
        /// </para>
        /// <para>
        /// Use the <b>:</b> (colon) character in an expression to dereference an attribute value.
        /// For example, suppose that you wanted to check whether the value of the <c>ProductStatus</c>
        /// attribute was one of the following: 
        /// </para>
        /// <para>
        ///  <c>Available | Backordered | Discontinued</c> 
        /// </para>
        /// <para>
        /// You would first need to specify <c>ExpressionAttributeValues</c> as follows:
        /// </para>
        /// <para>
        ///  <c>{ ":avail":{"S":"Available"}, ":back":{"S":"Backordered"}, ":disc":{"S":"Discontinued"}
        /// }</c> 
        /// </para>
        /// <para>
        /// You could then use these values in an expression, such as this:
        /// </para>
        /// <para>
        ///  <c>ProductStatus IN (:avail, :back, :disc)</c> 
        /// </para>
        /// <para>
        /// For more information on expression attribute values, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.SpecifyingConditions.html">Condition
        /// Expressions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public Dictionary<string, AttributeValue>? ExpressionAttributeValues { get; set; }

        /// <summary>
        /// Filter expression
        /// <para>
        /// A string that contains conditions that DynamoDB applies after the <c>Scan</c> operation,
        /// but before the data is returned to you. Items that do not satisfy the <c>FilterExpression</c>
        /// criteria are not returned.
        /// </para>
        /// <para>
        /// A <c>FilterExpression</c> is applied after the items have already been read; the process
        /// of filtering does not consume any additional read capacity units.
        /// </para>
        /// <para>
        /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Scan.html#Scan.FilterExpression">Filter
        /// Expressions</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? FilterExpression { get; set; }

        /// <summary>
        /// Limit
        /// <para>
        /// The maximum number of items to evaluate (not necessarily the number of matching items).
        /// If DynamoDB processes the number of items up to the limit while processing the results,
        /// it stops the operation and returns the matching values up to that point, and a key
        /// in <c>LastEvaluatedKey</c> to apply in a subsequent operation, so that you can pick
        /// up where you left off. Also, if the processed dataset size exceeds 1 MB before DynamoDB
        /// reaches this limit, it stops the operation and returns the matching values up to the
        /// limit, and a key in <c>LastEvaluatedKey</c> to apply in a subsequent operation to
        /// continue the operation. For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/QueryAndScan.html">Working
        /// with Queries</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Projection expression
        /// <para>
        /// A string that identifies one or more attributes to retrieve from the specified table
        /// or index. These attributes can include scalars, sets, or elements of a JSON document.
        /// The attributes in the expression must be separated by commas.
        /// </para>
        /// <para>
        /// If no attribute names are specified, then all attributes will be returned. If any
        /// of the requested attributes are not found, they will not appear in the result.
        /// </para>
        /// <para>
        /// For more information, see <a href="https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Expressions.AccessingItemAttributes.html">Specifying
        /// Item Attributes</a> in the <i>Amazon DynamoDB Developer Guide</i>.
        /// </para>
        /// </summary>
        public string? ProjectionExpression { get; set; }

        /// <summary>
        /// Segment
        /// <para>
        /// For a parallel <c>Scan</c> request, <c>Segment</c> identifies an individual segment
        /// to be scanned by an application worker.
        /// </para>
        /// <para>
        /// Segment IDs are zero-based, so the first segment is always 0. For example, if you
        /// want to use four application threads to scan a table or an index, then the first thread
        /// specifies a <c>Segment</c> value of 0, the second thread specifies 1, and so on.
        /// </para>
        /// <para>
        /// The value of <c>LastEvaluatedKey</c> returned from a parallel <c>Scan</c> request
        /// must be used as <c>ExclusiveStartKey</c> with the same segment ID in a subsequent
        /// <c>Scan</c> operation.
        /// </para>
        /// <para>
        /// The value for <c>Segment</c> must be greater than or equal to 0, and less than the
        /// value provided for <c>TotalSegments</c>.
        /// </para>
        /// <para>
        /// If you provide <c>Segment</c>, you must also provide <c>TotalSegments</c>.
        /// </para>
        /// </summary>
        public int? Segment { get; set; }

        /// <summary>
        /// Select
        /// <para>
        /// The attributes to be returned in the result. You can retrieve all item attributes,
        /// specific item attributes, the count of matching items, or in the case of an index,
        /// some or all of the attributes projected into the index.
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <c>ALL_ATTRIBUTES</c> - Returns all of the item attributes from the specified table
        /// or index. If you query a local secondary index, then for each matching item in the
        /// index, DynamoDB fetches the entire item from the parent table. If the index is configured
        /// to project all item attributes, then all of the data can be obtained from the local
        /// secondary index, and no fetching is required.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>ALL_PROJECTED_ATTRIBUTES</c> - Allowed only when querying an index. Retrieves
        /// all attributes that have been projected into the index. If the index is configured
        /// to project all attributes, this return value is equivalent to specifying <c>ALL_ATTRIBUTES</c>.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>COUNT</c> - Returns the number of matching items, rather than the matching items
        /// themselves. Note that this uses the same quantity of read capacity units as getting
        /// the items, and is subject to the same item size calculations.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <c>SPECIFIC_ATTRIBUTES</c> - Returns only the attributes listed in <c>ProjectionExpression</c>.
        /// This return value is equivalent to specifying <c>ProjectionExpression</c> without
        /// specifying any value for <c>Select</c>.
        /// </para>
        /// <para>
        /// If you query or scan a local secondary index and request only attributes that are
        /// projected into that index, the operation reads only the index and not the table. If
        /// any of the requested attributes are not projected into the local secondary index,
        /// DynamoDB fetches each of these attributes from the parent table. This extra fetching
        /// incurs additional throughput cost and latency.
        /// </para>
        /// <para>
        /// If you query or scan a global secondary index, you can only request attributes that
        /// are projected into the index. Global secondary index queries cannot fetch attributes
        /// from the parent table.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// If neither <c>Select</c> nor <c>ProjectionExpression</c> are specified, DynamoDB defaults
        /// to <c>ALL_ATTRIBUTES</c> when accessing a table, and <c>ALL_PROJECTED_ATTRIBUTES</c>
        /// when accessing an index. You cannot use both <c>Select</c> and <c>ProjectionExpression</c>
        /// together in a single request, unless the value for <c>Select</c> is <c>SPECIFIC_ATTRIBUTES</c>.
        /// (This usage is equivalent to specifying <c>ProjectionExpression</c> without any value
        /// for <c>Select</c>.)
        /// </para>
        /// <para>
        /// If you use the <c>ProjectionExpression</c> parameter, then the value for <c>Select</c>
        /// can only be <c>SPECIFIC_ATTRIBUTES</c>. Any other value for <c>Select</c> will return
        /// an error.
        /// </para>
        /// </summary>
        public Select? Select { get; set; }

        /// <summary>
        /// Total segments
        /// <para>
        /// For a parallel <c>Scan</c> request, <c>TotalSegments</c> represents the total number
        /// of segments into which the <c>Scan</c> operation will be divided. The value of <c>TotalSegments</c>
        /// corresponds to the number of application workers that will perform the parallel scan.
        /// For example, if you want to use four application threads to scan a table or an index,
        /// specify a <c>TotalSegments</c> value of 4.
        /// </para>
        /// <para>
        /// The value for <c>TotalSegments</c> must be greater than or equal to 1, and less than
        /// or equal to 1000000. If you specify a <c>TotalSegments</c> value of 1, the <c>Scan</c>
        /// operation will be sequential rather than parallel.
        /// </para>
        /// <para>
        /// If you specify <c>TotalSegments</c>, you must also specify <c>Segment</c>.
        /// </para>
        /// </summary>
        public int? TotalSegments { get; set; }

        /// <summary>Get request parameters for XRay</summary>
        public override Dictionary<string, object?> GetXRayRequestParameters()
        {
            return new Dictionary<string, object?>
            {
                { "table_name", TableName },
                { "index_name", IndexName },
                { "consistent_read", ConsistentRead.ToString() },
                { "projection_expression", ProjectionExpression },
                { "limit", Limit.ToString() },
            };
        }
    }

    /// <inheritdoc cref="ScanRequest"/>
    public class ScanRequest<TEntity>
        where TEntity : class, IDynamoDbScanableEntity<TEntity>
    {
        /// <inheritdoc cref="AmazonWebServiceRequest.UserId"/>
        public required Guid? UserId { get; set; }

        /// <inheritdoc cref="ScanRequest.ExclusiveStartKey"/>
        public Dictionary<string, AttributeValue>? ExclusiveStartKey { get; set; }


        /// <summary>
        /// Filters applied after the <c>Scan</c>
        /// operation, but before the data is returned to you. Items that do not satisfy the criteria are not returned.
        /// <para>
        /// The following implementations can be used:
        /// <list type="table">
        /// <item><term>Comparisons</term><description>
        /// <see cref="EqualExpression"/>, <see cref="NotEqualExpression"/>, <see cref="ComparatorExpression"/>, <see cref="BetweenExpression"/>, <see cref="InExpression"/>
        /// </description></item>
        /// <item><term>Functions</term><description>
        /// <see cref="AttributeExistsExpression"/>, <see cref="AttributeNotExistsExpression"/>, <see cref="AttributeTypeExpression"/>, <see cref="BeginsWithExpression"/>, <see cref="ContainsExpression"/>
        /// </description></item>
        /// <item><term>Logical evaluations</term><description>
        /// <see cref="AndExpression"/>, <see cref="OrExpression"/>, <see cref="NotExpression"/>
        /// </description></item>
        /// <item><term>Parentheses</term><description>
        /// <see cref="ParenthesesExpression"/>
        /// </description></item>
        /// </list>
        /// </para>
        /// </summary>
        /// <remarks>Not implemented: <see cref="SizeExpression"/></remarks>
        public IFilterExpression? Filters { get; set; }


        /// <inheritdoc cref="AmazonDynamoDBRequest.ReturnConsumedCapacity"/>
        public ReturnConsumedCapacity? ReturnConsumedCapacity { get; set; }

        /// <inheritdoc cref="ScanRequest.ConsistentRead"/>
        public bool ConsistentRead { get; set; }

        /// <inheritdoc cref="ScanRequest.Limit"/>
        public int? Limit { get; set; }

        /// <inheritdoc cref="ScanRequest.Segment"/>
        public int? Segment { get; set; }

        /// <inheritdoc cref="ScanRequest.Select"/>
        public Select? Select { get; set; }

        /// <inheritdoc cref="ScanRequest.TotalSegments"/>
        public int? TotalSegments { get; set; }

        internal ScanRequest Build()
        {
            return new ScanRequest(UserId)
            {
                ReturnConsumedCapacity = ReturnConsumedCapacity,

                TableName = TEntity.TableName,
                IndexName = TEntity.IndexName,
                ConsistentRead = ConsistentRead,
                ExclusiveStartKey = ExclusiveStartKey,
                ExpressionAttributeNames = (Filters?.AttributeNames ?? [])
                    .Union(TEntity.ProjectedAttributes?.Select(x => new KeyValuePair<string, string>($"#{x}", x)) ?? [])
                    .ToDictionary()
                    .NullIfEmpty(),
                ExpressionAttributeValues = Filters?.AttributeValues.ToDictionary().NullIfEmpty(),
                FilterExpression = Filters?.Expression,
                Limit = Limit,
                ProjectionExpression = TEntity.ProjectedAttributes == null ? null : new StringBuilder().AppendJoin(", ", TEntity.ProjectedAttributes.Select(x => $"#{x}")).ToString(),
                Segment = Segment,
                Select = Select,
                TotalSegments = TotalSegments,
            };
        }
    }
}
