using System;
using System.Net;
using System.Text;

namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// This exception is thrown when there is a parse error on the response back from AWS.
    /// </summary>
    public class AmazonUnmarshallingException : AmazonServiceException
    {
        #region Constructors

        public AmazonUnmarshallingException(string requestId, string lastKnownLocation, Exception innerException)
            : base("Error unmarshalling response back from AWS.", innerException)
        {
            this.RequestId = requestId;
            this.LastKnownLocation = lastKnownLocation;
        }

        public AmazonUnmarshallingException(string requestId, string lastKnownLocation, string responseBody, Exception innerException)
            : base("Error unmarshalling response back from AWS.", innerException)
        {
            this.RequestId = requestId;
            this.LastKnownLocation = lastKnownLocation;
            this.ResponseBody = responseBody;
        }

        public AmazonUnmarshallingException(string requestId, string lastKnownLocation,
            string responseBody, string message, Exception innerException)
            : base("Error unmarshalling response back from AWS. " + message, innerException)
        {
            this.RequestId = requestId;
            this.LastKnownLocation = lastKnownLocation;
            this.ResponseBody = responseBody;
        }

        public AmazonUnmarshallingException(string requestId, string lastKnownLocation, Exception innerException, HttpStatusCode statusCode)
            : base("Error unmarshalling response back from AWS.", innerException, statusCode)
        {
            this.RequestId = requestId;
            this.LastKnownLocation = lastKnownLocation;
        }

        public AmazonUnmarshallingException(string requestId, string lastKnownLocation, string responseBody, Exception innerException, HttpStatusCode statusCode)
            : base("Error unmarshalling response back from AWS.", innerException, statusCode)
        {
            this.RequestId = requestId;
            this.LastKnownLocation = lastKnownLocation;
            this.ResponseBody = responseBody;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Last known location in the response that was parsed, if available.
        /// </summary>
        public string LastKnownLocation { get; private set; }

        /// <summary>
        /// The entire response body that caused this exception, if available.
        /// </summary>
        public string ResponseBody { get; private set; }

        #endregion

        #region Overrides

        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                AppendFormat(sb, "Request ID: {0}", this.RequestId);
                AppendFormat(sb, "Response Body: {0}", this.ResponseBody);
                AppendFormat(sb, "Last Parsed Path: {0}", this.LastKnownLocation);
                AppendFormat(sb, "HTTP Status Code: {0}", (int)(this.StatusCode) + " " + this.StatusCode.ToString());

                var partialMessage = sb.ToString();

                return base.Message + " " + partialMessage;
            }
        }

        #endregion

        #region Private methods

        private static void AppendFormat(StringBuilder sb, string format, string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (sb.Length > 0)
                sb.Append(", ");

            sb.AppendFormat(format, value);
        }

        #endregion
    }
}
