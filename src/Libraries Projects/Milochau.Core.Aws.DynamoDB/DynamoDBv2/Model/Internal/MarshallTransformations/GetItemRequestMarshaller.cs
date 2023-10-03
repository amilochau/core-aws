﻿using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal;
using Amazon.Runtime;
using System.Globalization;
using System.IO;
using ThirdParty.Json.LitJson;

// https://github.com/aws/aws-sdk-net/blob/master/sdk/src/Services/DynamoDBv2/Generated/Model/Internal/MarshallTransformations/GetItemRequestMarshaller.cs
namespace Milochau.Core.Aws.DynamoDB.DynamoDBv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// GetItem Request Marshaller
    /// </summary>       
    public class GetItemRequestMarshaller : IMarshaller<IRequest, GetItemRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
    {
        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="input"></param>
        /// <returns></returns>
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((GetItemRequest)input);
        }

        /// <summary>
        /// Marshaller the request object to the HTTP request.
        /// </summary>  
        /// <param name="publicRequest"></param>
        /// <returns></returns>
        public IRequest Marshall(GetItemRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.DynamoDBv2");
            string target = "DynamoDB_20120810.GetItem";
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
                if (publicRequest.IsSetAttributesToGet())
                {
                    context.Writer.WritePropertyName("AttributesToGet");
                    context.Writer.WriteArrayStart();
                    foreach (var publicRequestAttributesToGetListValue in publicRequest.AttributesToGet)
                    {
                        context.Writer.Write(publicRequestAttributesToGetListValue);
                    }
                    context.Writer.WriteArrayEnd();
                }

                if (publicRequest.IsSetConsistentRead())
                {
                    context.Writer.WritePropertyName("ConsistentRead");
                    context.Writer.Write(publicRequest.ConsistentRead);
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

                if (publicRequest.IsSetKey())
                {
                    context.Writer.WritePropertyName("Key");
                    context.Writer.WriteObjectStart();
                    foreach (var publicRequestKeyKvp in publicRequest.Key)
                    {
                        context.Writer.WritePropertyName(publicRequestKeyKvp.Key);
                        var publicRequestKeyValue = publicRequestKeyKvp.Value;

                        context.Writer.WriteObjectStart();

                        var marshaller = AttributeValueMarshaller.Instance;
                        marshaller.Marshall(publicRequestKeyValue, context);

                        context.Writer.WriteObjectEnd();
                    }
                    context.Writer.WriteObjectEnd();
                }

                if (publicRequest.IsSetProjectionExpression())
                {
                    context.Writer.WritePropertyName("ProjectionExpression");
                    context.Writer.Write(publicRequest.ProjectionExpression);
                }

                if (publicRequest.IsSetReturnConsumedCapacity())
                {
                    context.Writer.WritePropertyName("ReturnConsumedCapacity");
                    context.Writer.Write(publicRequest.ReturnConsumedCapacity);
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
        private static GetItemRequestMarshaller _instance = new GetItemRequestMarshaller();

        internal static GetItemRequestMarshaller GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static GetItemRequestMarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
