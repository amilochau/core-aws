using Milochau.Core.Aws.Core.Runtime.Internal.Transform;
using Milochau.Core.Aws.Core.Runtime;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Milochau.Core.Aws.SNS.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for Publish operation
    /// </summary>  
    public static class PublishResponseUnmarshaller
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>
        public static PublishResponse UnmarshallResponse(JsonUnmarshallerContext context)
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
        public static AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, HttpStatusCode statusCode)
        {
            var errorResponse = JsonErrorResponseUnmarshaller.Instance.UnmarshallResponse(context);
            errorResponse.StatusCode = statusCode;

            return new AmazonSimpleNotificationServiceException(errorResponse.Message, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        }
    }
}
