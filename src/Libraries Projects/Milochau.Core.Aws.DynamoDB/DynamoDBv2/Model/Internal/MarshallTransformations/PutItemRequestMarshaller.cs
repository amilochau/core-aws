using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;
using System.Globalization;
using System.IO;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/PutItemRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// PutItem Request Marshaller
    /// </summary>       
    public class PutItemRequestMarshaller : IMarshaller<IRequest, PutItemRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((PutItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public IRequest Marshall(PutItemRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.DynamoDBv2");
            string target = "DynamoDB_20120810.PutItem";
            request.Headers["X-Amz-Target"] = target;
            request.Headers["Content-Type"] = "application/x-amz-json-1.0";
            request.Headers[Amazon.Util.HeaderKeys.XAmzApiVersion] = "2012-08-10";
            request.HttpMethod = "POST";

            request.ResourcePath = "/";
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonWriter(stringWriter);
                writer.WriteObjectStart();
                if (publicRequest.IsSetConditionalOperator())
                {
                    writer.WritePropertyName("ConditionalOperator");
                    writer.Write(publicRequest.ConditionalOperator!.Value);
                }

                if (publicRequest.IsSetConditionExpression())
                {
                    writer.WritePropertyName("ConditionExpression");
                    writer.Write(publicRequest.ConditionExpression);
                }

                if (publicRequest.IsSetExpected())
                {
                    writer.WritePropertyName("Expected");
                    writer.WriteObjectStart();
                    foreach (var publicRequestExpectedKvp in publicRequest.Expected)
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

                if (publicRequest.IsSetExpressionAttributeNames())
                {
                    writer.WritePropertyName("ExpressionAttributeNames");
                    writer.WriteObjectStart();
                    foreach (var publicRequestExpressionAttributeNamesKvp in publicRequest.ExpressionAttributeNames)
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
                    foreach (var publicRequestExpressionAttributeValuesKvp in publicRequest.ExpressionAttributeValues)
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

                if (publicRequest.IsSetItem())
                {
                    writer.WritePropertyName("Item");
                    writer.WriteObjectStart();
                    foreach (var publicRequestItemKvp in publicRequest.Item)
                    {
                        writer.WritePropertyName(publicRequestItemKvp.Key);
                        var publicRequestItemValue = publicRequestItemKvp.Value;

                        writer.WriteObjectStart();

                        var marshaller = AttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestItemValue, writer);

                        writer.WriteObjectEnd();
                    }
                    writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetReturnConsumedCapacity())
                {
                    writer.WritePropertyName("ReturnConsumedCapacity");
                    writer.Write(publicRequest.ReturnConsumedCapacity!.Value);
                }

                if (publicRequest.IsSetReturnItemCollectionMetrics())
                {
                    writer.WritePropertyName("ReturnItemCollectionMetrics");
                    writer.Write(publicRequest.ReturnItemCollectionMetrics!.Value);
                }

                if (publicRequest.IsSetReturnValues())
                {
                    writer.WritePropertyName("ReturnValues");
                    writer.Write(publicRequest.ReturnValues!.Value);
                }

                if (publicRequest.IsSetReturnValuesOnConditionCheckFailure())
                {
                    writer.WritePropertyName("ReturnValuesOnConditionCheckFailure");
                    writer.Write(publicRequest.ReturnValuesOnConditionCheckFailure!.Value);
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
        public static PutItemRequestMarshaller Instance { get; } = new PutItemRequestMarshaller();
    }
}
