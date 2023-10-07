using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;
using System.IO;
using System.Globalization;
using ThirdParty.Json.LitJson;

namespace Milochau.Core.Aws.DynamoDB.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// UpdateItem Request Marshaller
    /// </summary>       
    public class UpdateItemRequestMarshaller : IMarshaller<IRequest, UpdateItemRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((UpdateItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <returns></returns>
        public IRequest Marshall(UpdateItemRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.DynamoDBv2");
            string target = "DynamoDB_20120810.UpdateItem";
            request.Headers["X-Amz-Target"] = target;
            request.Headers["Content-Type"] = "application/x-amz-json-1.0";
            request.Headers[Amazon.Util.HeaderKeys.XAmzApiVersion] = "2012-08-10";
            request.HttpMethod = "POST";

            request.ResourcePath = "/";
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonWriter(stringWriter);
                writer.WriteObjectStart();
                if(publicRequest.IsSetAttributeUpdates())
                {
                    writer.WritePropertyName("AttributeUpdates");
                    writer.WriteObjectStart();
                    foreach (var publicRequestAttributeUpdatesKvp in publicRequest.AttributeUpdates!)
                    {
                        writer.WritePropertyName(publicRequestAttributeUpdatesKvp.Key);
                        var publicRequestAttributeUpdatesValue = publicRequestAttributeUpdatesKvp.Value;

                        writer.WriteObjectStart();

                        var marshaller = AttributeValueUpdateMarshaller.Instance;
                        marshaller.Marshall(publicRequestAttributeUpdatesValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteObjectEnd();
                }

                if(publicRequest.IsSetConditionalOperator())
                {
                    writer.WritePropertyName("ConditionalOperator");
                    writer.Write(publicRequest.ConditionalOperator!.Value);
                }

                if(publicRequest.IsSetConditionExpression())
                {
                    writer.WritePropertyName("ConditionExpression");
                    writer.Write(publicRequest.ConditionExpression);
                }

                if(publicRequest.IsSetExpected())
                {
                    writer.WritePropertyName("Expected");
                    writer.WriteObjectStart();
                    foreach (var publicRequestExpectedKvp in publicRequest.Expected!)
                    {
                        writer.WritePropertyName(publicRequestExpectedKvp.Key);
                        var publicRequestExpectedValue = publicRequestExpectedKvp.Value;

                        writer.WriteObjectStart();

                        var marshaller = ExpectedAttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestExpectedValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteObjectEnd();
                }

                if(publicRequest.IsSetExpressionAttributeNames())
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

                if(publicRequest.IsSetExpressionAttributeValues())
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

                if(publicRequest.IsSetKey())
                {
                    writer.WritePropertyName("Key");
                    writer.WriteObjectStart();
                    foreach (var publicRequestKeyKvp in publicRequest.Key!)
                    {
                        writer.WritePropertyName(publicRequestKeyKvp.Key);
                        var publicRequestKeyValue = publicRequestKeyKvp.Value;

                        writer.WriteObjectStart();

                        var marshaller = AttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestKeyValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteObjectEnd();
                }

                if(publicRequest.IsSetReturnConsumedCapacity())
                {
                    writer.WritePropertyName("ReturnConsumedCapacity");
                    writer.Write(publicRequest.ReturnConsumedCapacity!.Value);
                }

                if(publicRequest.IsSetReturnItemCollectionMetrics())
                {
                    writer.WritePropertyName("ReturnItemCollectionMetrics");
                    writer.Write(publicRequest.ReturnItemCollectionMetrics!.Value);
                }

                if(publicRequest.IsSetReturnValues())
                {
                    writer.WritePropertyName("ReturnValues");
                    writer.Write(publicRequest.ReturnValues!.Value);
                }

                if(publicRequest.IsSetReturnValuesOnConditionCheckFailure())
                {
                    writer.WritePropertyName("ReturnValuesOnConditionCheckFailure");
                    writer.Write(publicRequest.ReturnValuesOnConditionCheckFailure!.Value);
                }

                if(publicRequest.IsSetTableName())
                {
                    writer.WritePropertyName("TableName");
                    writer.Write(publicRequest.TableName);
                }

                if(publicRequest.IsSetUpdateExpression())
                {
                    writer.WritePropertyName("UpdateExpression");
                    writer.Write(publicRequest.UpdateExpression);
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
        public static UpdateItemRequestMarshaller Instance { get; } = new UpdateItemRequestMarshaller();
    }
}
