using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;
using System.Globalization;
using System.IO;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Query Request Marshaller
    /// </summary>       
    public class QueryRequestMarshaller : IMarshaller<IRequest, QueryRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((QueryRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        public IRequest Marshall(QueryRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.DynamoDBv2");
            string target = "DynamoDB_20120810.Query";
            request.Headers["X-Amz-Target"] = target;
            request.Headers["Content-Type"] = "application/x-amz-json-1.0";
            request.Headers[Amazon.Util.HeaderKeys.XAmzApiVersion] = "2012-08-10";
            request.HttpMethod = "POST";

            request.ResourcePath = "/";
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonWriter(stringWriter);
                writer.WriteObjectStart();
                if (publicRequest.IsSetAttributesToGet())
                {
                    writer.WritePropertyName("AttributesToGet");
                    writer.WriteArrayStart();
                    foreach (var publicRequestAttributesToGetListValue in publicRequest.AttributesToGet!)
                    {
                        writer.Write(publicRequestAttributesToGetListValue);
                    }
                    writer.WriteArrayEnd();
                }

                if (publicRequest.IsSetConditionalOperator())
                {
                    writer.WritePropertyName("ConditionalOperator");
                    writer.Write(publicRequest.ConditionalOperator!.Value);
                }

                if (publicRequest.IsSetConsistentRead())
                {
                    writer.WritePropertyName("ConsistentRead");
                    writer.Write(publicRequest.ConsistentRead);
                }

                if (publicRequest.IsSetExclusiveStartKey())
                {
                    writer.WritePropertyName("ExclusiveStartKey");
                    writer.WriteObjectStart();
                    foreach (var publicRequestExclusiveStartKeyKvp in publicRequest.ExclusiveStartKey!)
                    {
                        writer.WritePropertyName(publicRequestExclusiveStartKeyKvp.Key);
                        var publicRequestExclusiveStartKeyValue = publicRequestExclusiveStartKeyKvp.Value;

                        writer.WriteObjectStart();

                        var marshaller = AttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestExclusiveStartKeyValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetExpressionAttributeNames())
                {
                    writer.WritePropertyName("ExpressionAttributeNames");
                    writer.WriteObjectStart();
                    foreach (var publicRequestExpressionAttributeNamesKvp in publicRequest.ExpressionAttributeNames!)
                    {
                        writer.WritePropertyName(publicRequestExpressionAttributeNamesKvp.Key);
                        var publicRequestExpressionAttributeNamesValue = publicRequestExpressionAttributeNamesKvp.Value;

                        writer.Write(publicRequestExpressionAttributeNamesValue);
                    }
                    writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetExpressionAttributeValues())
                {
                    writer.WritePropertyName("ExpressionAttributeValues");
                    writer.WriteObjectStart();
                    foreach (var publicRequestExpressionAttributeValuesKvp in publicRequest.ExpressionAttributeValues!)
                    {
                        writer.WritePropertyName(publicRequestExpressionAttributeValuesKvp.Key);
                        var publicRequestExpressionAttributeValuesValue = publicRequestExpressionAttributeValuesKvp.Value;

                        writer.WriteObjectStart();

                        var marshaller = AttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestExpressionAttributeValuesValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetFilterExpression())
                {
                    writer.WritePropertyName("FilterExpression");
                    writer.Write(publicRequest.FilterExpression);
                }

                if (publicRequest.IsSetIndexName())
                {
                    writer.WritePropertyName("IndexName");
                    writer.Write(publicRequest.IndexName);
                }

                if (publicRequest.IsSetKeyConditionExpression())
                {
                    writer.WritePropertyName("KeyConditionExpression");
                    writer.Write(publicRequest.KeyConditionExpression);
                }

                if (publicRequest.IsSetKeyConditions())
                {
                    writer.WritePropertyName("KeyConditions");
                    writer.WriteObjectStart();
                    foreach (var publicRequestKeyConditionsKvp in publicRequest.KeyConditions!)
                    {
                        writer.WritePropertyName(publicRequestKeyConditionsKvp.Key);
                        var publicRequestKeyConditionsValue = publicRequestKeyConditionsKvp.Value;

                        writer.WriteObjectStart();

                        var marshaller = ConditionMarshaller.Instance;
                        marshaller.Marshall(publicRequestKeyConditionsValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteObjectEnd();
                }

                if (publicRequest.Limit.HasValue)
                {
                    writer.WritePropertyName("Limit");
                    writer.Write(publicRequest.Limit.Value);
                }

                if (publicRequest.IsSetProjectionExpression())
                {
                    writer.WritePropertyName("ProjectionExpression");
                    writer.Write(publicRequest.ProjectionExpression);
                }

                if (publicRequest.IsSetQueryFilter())
                {
                    writer.WritePropertyName("QueryFilter");
                    writer.WriteObjectStart();
                    foreach (var publicRequestQueryFilterKvp in publicRequest.QueryFilter!)
                    {
                        writer.WritePropertyName(publicRequestQueryFilterKvp.Key);
                        var publicRequestQueryFilterValue = publicRequestQueryFilterKvp.Value;

                        writer.WriteObjectStart();

                        var marshaller = ConditionMarshaller.Instance;
                        marshaller.Marshall(publicRequestQueryFilterValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetReturnConsumedCapacity())
                {
                    writer.WritePropertyName("ReturnConsumedCapacity");
                    writer.Write(publicRequest.ReturnConsumedCapacity!.Value);
                }

                if (publicRequest.IsSetScanIndexForward())
                {
                    writer.WritePropertyName("ScanIndexForward");
                    writer.Write(publicRequest.ScanIndexForward);
                }

                if (publicRequest.IsSetSelect())
                {
                    writer.WritePropertyName("Select");
                    writer.Write(publicRequest.Select!.Value);
                }

                if (publicRequest.IsSetTableName())
                {
                    writer.WritePropertyName("TableName");
                    writer.Write(publicRequest.TableName);
                }

                writer.WriteObjectEnd();
                string snippet = stringWriter.ToString();
                request.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }


            return request;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static QueryRequestMarshaller Instance { get; } = new QueryRequestMarshaller();
    }
}
