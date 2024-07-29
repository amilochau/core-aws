using Milochau.Core.Aws.Core.References;
using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using Milochau.Core.Aws.Core.Runtime.Internal;

namespace Milochau.Core.Aws.SNS.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Publish Request Marshaller
    /// </summary>
    internal class PublishInvokeOptions : IInvokeOptions<PublishRequest, PublishResponse>
    {
        public string MonitoringOriginalRequestName { get; } = "Publish";

        /// <summary>Creates an HTTP request message to call the service</summary>
        public HttpRequestMessage MarshallRequest(PublishRequest publicRequest)
        {
            var uriBuilder = new UriBuilder($"https://sns.{EnvironmentVariables.RegionName}.amazonaws.com"); //Action=Publish&Version=2010-03-31");

            var contentDictionary = new Dictionary<string, string>
            {
                { "Action", "Publish" },
                { "Version", "2010-03-31" },
                { "TopicArn", publicRequest.TopicArn },
                { "Message", publicRequest.Message },
            };

            uriBuilder.Query += $"&TopicArn={HttpUtility.UrlEncode(publicRequest.TopicArn)}";
            if (!string.IsNullOrEmpty(publicRequest.MessageDeduplicationId))
            {
                uriBuilder.Query += $"&MessageDeduplicationId={HttpUtility.UrlEncode(publicRequest.MessageDeduplicationId)}";
                contentDictionary.Add("MessageDeduplicationId", publicRequest.MessageDeduplicationId);
            }
            if (!string.IsNullOrEmpty(publicRequest.MessageGroupId))
            {
                uriBuilder.Query += $"&MessageGroupId={HttpUtility.UrlEncode(publicRequest.MessageGroupId)}";
                contentDictionary.Add("MessageGroupId", publicRequest.MessageGroupId);
            }
            if (!string.IsNullOrEmpty(publicRequest.MessageStructure))
            {
                uriBuilder.Query += $"&MessageStructure={HttpUtility.UrlEncode(publicRequest.MessageStructure)}";
                contentDictionary.Add("MessageStructure", publicRequest.MessageStructure);
            }
            if (!string.IsNullOrEmpty(publicRequest.Subject))
            {
                uriBuilder.Query += $"&Subject={HttpUtility.UrlEncode(publicRequest.Subject)}";
                contentDictionary.Add("Subject", publicRequest.Subject);
            }
            if (publicRequest.MessageAttributes != null)
            {
                var mapIndex = 1;
                foreach (var key in publicRequest.MessageAttributes.Keys)
                {
                    uriBuilder.Query += $"MessageAttributes.entry.{mapIndex}.Name={key}";
                    contentDictionary.Add($"MessageAttributes.entry.{mapIndex}.Name", key);
                    if (publicRequest.MessageAttributes.TryGetValue(key, out var value))
                    {
                        if (value.DataType != null)
                        {
                            uriBuilder.Query += $"&MessageAttributes.entry.{mapIndex}.Value.DataType={HttpUtility.UrlEncode(value.DataType)}";
                            contentDictionary.Add($"MessageAttributes.entry.{mapIndex}.Value.DataType", value.DataType);
                        }
                        if (value.StringValue != null)
                        {
                            uriBuilder.Query += $"&MessageAttributes.entry.{mapIndex}.Value.StringValue={HttpUtility.UrlEncode(value.StringValue)}";
                            contentDictionary.Add($"MessageAttributes.entry.{mapIndex}.Value.StringValue", value.StringValue);
                        }
                    }
                    mapIndex++;
                }
            }

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://sns.{EnvironmentVariables.RegionName}.amazonaws.com"),
                Content = new FormUrlEncodedContent(contentDictionary),
            };

            return httpRequestMessage;
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>
        public PublishResponse UnmarshallResponse(JsonUnmarshallerContext context)
        {
            var response = new PublishResponse();

            using var streamReader = new StreamReader(context.Stream);
            var content = streamReader.ReadToEnd();

            var messageIdRegexMatch = Regex.Match(content, "<MessageId>([0-9a-f-]*)<\\/MessageId>");
            if (messageIdRegexMatch.Success)
            {
                response.MessageId = messageIdRegexMatch.Groups[1].Value;
            }

            var sequenceNumberRegexMatch = Regex.Match(content, "<SequenceNumber>([0-9a-f-]*)<\\/MessageId>");
            if (sequenceNumberRegexMatch.Success)
            {
                response.SequenceNumber = sequenceNumberRegexMatch.Groups[1].Value;
            }

            return response;
        }

        /// <summary>
        /// Unmarshaller error response to exception.
        /// </summary>
        public AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.UnmarshallResponse(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonSimpleNotificationServiceException(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }
    }
}
