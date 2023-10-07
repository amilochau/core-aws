using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;
using System.Globalization;
using System.IO;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/BatchWriteItemRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// BatchWriteItem Request Marshaller
    /// </summary>       
    public class BatchWriteItemRequestMarshaller : IMarshaller<IRequest, BatchWriteItemRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((BatchWriteItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public IRequest Marshall(BatchWriteItemRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.DynamoDBv2");
            string target = "DynamoDB_20120810.BatchWriteItem";
            request.Headers["X-Amz-Target"] = target;
            request.Headers["Content-Type"] = "application/x-amz-json-1.0";
            request.Headers[Amazon.Util.HeaderKeys.XAmzApiVersion] = "2012-08-10";
            request.HttpMethod = "POST";

            request.ResourcePath = "/";
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonWriter(stringWriter);
                writer.WriteObjectStart();
                if (publicRequest.IsSetRequestItems())
                {
                    writer.WritePropertyName("RequestItems");
                    writer.WriteObjectStart();
                    foreach (var publicRequestRequestItemsKvp in publicRequest.RequestItems)
                    {
                        writer.WritePropertyName(publicRequestRequestItemsKvp.Key);
                        var publicRequestRequestItemsValue = publicRequestRequestItemsKvp.Value;

                        writer.WriteArrayStart();
                        foreach (var publicRequestRequestItemsValueListValue in publicRequestRequestItemsValue)
                        {
                            writer.WriteObjectStart();

                            var marshaller = WriteRequestMarshaller.Instance;
                            marshaller.Marshall(publicRequestRequestItemsValueListValue, writer);

                            writer.WriteObjectEnd();
                        }
                        writer.WriteArrayEnd();
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

                writer.WriteObjectEnd();
                string snippet = stringWriter.ToString();
                request.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }


            return request;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static BatchWriteItemRequestMarshaller Instance { get; } = new BatchWriteItemRequestMarshaller();
    }
}
