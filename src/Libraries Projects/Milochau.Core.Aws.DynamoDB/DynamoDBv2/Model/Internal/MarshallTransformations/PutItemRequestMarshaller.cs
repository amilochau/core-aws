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
                var context = new JsonMarshallerContext(request, writer);
                if (publicRequest.IsSetConditionalOperator())
                {
                    context.Writer.WritePropertyName("ConditionalOperator");
                    context.Writer.Write(publicRequest.ConditionalOperator!.Value);
                }

                if (publicRequest.IsSetConditionExpression())
                {
                    context.Writer.WritePropertyName("ConditionExpression");
                    context.Writer.Write(publicRequest.ConditionExpression);
                }

                if (publicRequest.IsSetExpected())
                {
                    context.Writer.WritePropertyName("Expected");
                    context.Writer.WriteObjectStart();
                    foreach (var publicRequestExpectedKvp in publicRequest.Expected)
                    {
                        context.Writer.WritePropertyName(publicRequestExpectedKvp.Key);
                        var publicRequestExpectedValue = publicRequestExpectedKvp.Value;

                        context.Writer.WriteObjectStart();

                        var marshaller = ExpectedAttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestExpectedValue, context);

                        context.Writer.WriteObjectEnd();
                    }
                    context.Writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetExpressionAttributeNames())
                {
                    context.Writer.WritePropertyName("ExpressionAttributeNames");
                    context.Writer.WriteObjectStart();
                    foreach (var publicRequestExpressionAttributeNamesKvp in publicRequest.ExpressionAttributeNames)
                    {
                        context.Writer.WritePropertyName(publicRequestExpressionAttributeNamesKvp.Key);
                        var publicRequestExpressionAttributeNamesValue = publicRequestExpressionAttributeNamesKvp.Value;

                        context.Writer.Write(publicRequestExpressionAttributeNamesValue);
                    }
                    context.Writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetExpressionAttributeValues())
                {
                    context.Writer.WritePropertyName("ExpressionAttributeValues");
                    context.Writer.WriteObjectStart();
                    foreach (var publicRequestExpressionAttributeValuesKvp in publicRequest.ExpressionAttributeValues)
                    {
                        context.Writer.WritePropertyName(publicRequestExpressionAttributeValuesKvp.Key);
                        var publicRequestExpressionAttributeValuesValue = publicRequestExpressionAttributeValuesKvp.Value;

                        context.Writer.WriteObjectStart();

                        var marshaller = AttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestExpressionAttributeValuesValue, context);

                        context.Writer.WriteObjectEnd();
                    }
                    context.Writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetItem())
                {
                    context.Writer.WritePropertyName("Item");
                    context.Writer.WriteObjectStart();
                    foreach (var publicRequestItemKvp in publicRequest.Item)
                    {
                        context.Writer.WritePropertyName(publicRequestItemKvp.Key);
                        var publicRequestItemValue = publicRequestItemKvp.Value;

                        context.Writer.WriteObjectStart();

                        var marshaller = AttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestItemValue, context);

                        context.Writer.WriteObjectEnd();
                    }
                    context.Writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetReturnConsumedCapacity())
                {
                    context.Writer.WritePropertyName("ReturnConsumedCapacity");
                    context.Writer.Write(publicRequest.ReturnConsumedCapacity!.Value);
                }

                if (publicRequest.IsSetReturnItemCollectionMetrics())
                {
                    context.Writer.WritePropertyName("ReturnItemCollectionMetrics");
                    context.Writer.Write(publicRequest.ReturnItemCollectionMetrics!.Value);
                }

                if (publicRequest.IsSetReturnValues())
                {
                    context.Writer.WritePropertyName("ReturnValues");
                    context.Writer.Write(publicRequest.ReturnValues!.Value);
                }

                if (publicRequest.IsSetReturnValuesOnConditionCheckFailure())
                {
                    context.Writer.WritePropertyName("ReturnValuesOnConditionCheckFailure");
                    context.Writer.Write(publicRequest.ReturnValuesOnConditionCheckFailure!.Value);
                }

                if (publicRequest.IsSetTableName())
                {
                    context.Writer.WritePropertyName("TableName");
                    context.Writer.Write(publicRequest.TableName);
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
